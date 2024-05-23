using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CC_Backend.Models;
using CC_Backend.Repositories.LikeRepo;
using CC_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CC_Backend.Data;


namespace CC_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LikeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILikeRepo _likeRepo;
        private readonly NatureAIContext _context;

        public LikeController(UserManager<ApplicationUser> usermanager, ILikeRepo likeRepo, NatureAIContext context)
        {
            _userManager = usermanager;
            _likeRepo = likeRepo;
            _context = context;
        }
        // POST: Like
        [HttpPost]
        [Authorize]
        [Route("/likes/addlike")]
        public async Task<IActionResult> AddLike([FromBody] LikeDTO likeDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var stampCollected = await _likeRepo.GetStampCollectedAsync(likeDto.StampCollectedId);
            if (stampCollected == null)
            {
                return BadRequest("The specified stamp collected does not exist.");
            }

            var hasAlreadyLiked = await _context.Likes
                         .AnyAsync(like => like.UserId == userId && like.StampCollectedId == stampCollected.StampCollectedId);
            if (!hasAlreadyLiked)
            {
                var like = new Like
                {
                    UserId = userId,
                    StampCollectedId = likeDto.StampCollectedId,
                    StampCollected = stampCollected,
                    CreatedAt = DateTime.Now

                };

                await _likeRepo.AddLikeAsync(like);



                return Ok(true);

            }
            return BadRequest("User has already liked this stamp.");
        }

        // DELETE: Like
        [HttpDelete]
        [Authorize]
        [Route("/like/deletelike")]
        public async Task<IActionResult> DeleteLike([FromBody] LikeDeleteDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var like = await _likeRepo.GetLikeByIdAsync(dto.LikeId);
            if (like == null)
            {
                return NotFound("Like wasn't found.");
            }



            await _likeRepo.DeleteLikeAsync(like);

            return Ok(true);
        }
    }
}
