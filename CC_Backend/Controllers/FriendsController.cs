using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Repositories.Friends;
using CC_Backend.Repositories.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Authorize]
        [Route("friends/getfriendsfromuser")]
        public async Task<IActionResult> GetFriendsFromUser()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();
                var result = await _iFriendRepo.GetFriendsAsync(userId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("friends/addfriend")]
        public async Task<IActionResult> AddFriend(AddFriendDTO dto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                string userId = user.Id.ToString();
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


        [HttpPost]
        [Authorize]
        [Route("friends/removefriend")]
        public async Task<IActionResult> RemoveFriend(RemoveFriendDTO dto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();
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
