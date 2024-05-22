using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Repositories.Friends;
using CC_Backend.Repositories.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CC_Backend.Controllers
{
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendsRepo _iFriendRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendsController(IFriendsRepo repo, UserManager<ApplicationUser> userManager)
        {
            _iFriendRepo = repo;
            _userManager = userManager;
        }

        // Get all friends from a user
        [HttpGet]
        [Authorize]
        [Route("friends/getfriendsfromuser")]
        public async Task<IActionResult> GetFriendsFromUser()
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var result = await _iFriendRepo.GetFriendsAsync(userId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Add a friend
        [HttpPost]
        [Authorize]
        [Route("friends/addfriend")]
        public async Task<IActionResult> AddFriend(AddFriendDTO dto)
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var (success, message) = await _iFriendRepo.AddFriendAsync(userId, dto);

                if (success)
                {
                    return Ok(message);
                }
                else
                {
                    return BadRequest(message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Remove a friend
        [HttpPost]
        [Authorize]
        [Route("friends/removefriend")]
        public async Task<IActionResult> RemoveFriend(RemoveFriendDTO dto)
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var (success, message) = await _iFriendRepo.RemoveFriendAsync(userId, dto);
                if (success)
                {
                    return Ok(message);
                }
                else
                {
                    return BadRequest(message);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
