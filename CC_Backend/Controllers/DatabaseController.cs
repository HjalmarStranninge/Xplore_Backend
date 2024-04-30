using CC_Backend.Data;
using CC_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace CC_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly IDBRepo _iDBRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseController(IDBRepo iDBRepo, UserManager<ApplicationUser> userManager)
        {
            _iDBRepo = iDBRepo;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("/getallusers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _iDBRepo.GetAllUsersAsync();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        [HttpPost]
        [Route("/getstampsfromuser")]

        public async Task<IActionResult> GetStampsFromUser()
        {            
            try
            {   
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();
                var result = await _iDBRepo.GetStampsFromUserAsync(userId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("/getfriendsfromuser")]
        public async Task<IActionResult> GetFriendsFromUser()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();
                var result = await _iDBRepo.GetFriendsAsync(userId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("/addfriend")]
        public async Task<IActionResult> AddFriend(string friendUserName)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();
                var (success, message) = await _iDBRepo.AddFriendAsync(userId, friendUserName);
                if (success)
                {
                    return Ok(message);
                }
                else
                {
                    return BadRequest(message);
                }
            }

            catch(Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }                
        }

        [HttpPost]
        [Route("/removefriend")]
        public async Task<IActionResult> RemoveFriend(string friendUserName)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();
                var (success, message) = await _iDBRepo.RemoveFriendAsync(userId, friendUserName);
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
