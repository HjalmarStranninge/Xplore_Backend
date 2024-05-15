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
    [Route("api/[controller]")]
    [Authorize]
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
        [Route("/getfriendsfromuser")]
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
        [Route("/addfriend")]
        public async Task<IActionResult> AddFriend(string friendDisplayName)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();
                var friendDTO = new AddFriendDTO
                {
                    FriendUserName = friendDisplayName,
                };

                var (success, message) = await _iFriendRepo.AddFriendAsync(userId, friendDTO);
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
        [Route("/removefriend")]
        public async Task<IActionResult> RemoveFriend(string friendDisplayName)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();
                var (success, message) = await _iFriendRepo.RemoveFriendAsync(userId, friendDisplayName);
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
