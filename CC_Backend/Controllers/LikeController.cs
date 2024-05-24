using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CC_Backend.Models;
using CC_Backend.Repositories.LikeRepo;
using CC_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CC_Backend.Data;
using CC_Backend.Repositories.Stamps;


namespace CC_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LikeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILikeRepo _likeRepo;
        private readonly IStampsRepo _stampsRepo;
        private readonly NatureAIContext _context;    

        public LikeController(UserManager<ApplicationUser> usermanager, ILikeRepo likeRepo,  IStampsRepo stampsRepo, NatureAIContext context)
        {
            _userManager = usermanager;
            _likeRepo = likeRepo;
            _stampsRepo = stampsRepo;
            _context = context;
        }

        // Like a stamp collected by a friend
        [HttpPost]
        [Authorize]
        [Route("/likes/addlike")]
        public async Task<IActionResult> AddLike([FromBody] LikeDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var stampCollected = await _stampsRepo.GetStampCollectedAsync(dto.StampCollectedId);
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
                    StampCollectedId = dto.StampCollectedId,
                    StampCollected = stampCollected,
                    CreatedAt = DateTime.Now

                };

                await _likeRepo.AddLikeAsync(like);

                return Ok(true);

            }
            return BadRequest("User has already liked this stamp.");

        }

        // Remove a like
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
