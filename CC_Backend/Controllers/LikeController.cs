using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CC_Backend.Models;
using CC_Backend.Repositories.LikeRepo;
using CC_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace CC_Backend.Controllers
{
        [ApiController]
        [Route("[controller]")]
    public class LikeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILikeRepo _likeRepo;

        public LikeController(UserManager<ApplicationUser> usermanager, ILikeRepo likeRepo)
        {
            _userManager = usermanager;
            _likeRepo = likeRepo;
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


            var like = new Like
            {
                StampCollectedId = likeDto.StampCollectedId,
                StampCollected = stampCollected,
                UserId = likeDto.UserId,
                CreatedAt = DateTime.Now

            };

            await _likeRepo.AddLikeAsync(like);



            return Ok(true);
        }
    }
}
