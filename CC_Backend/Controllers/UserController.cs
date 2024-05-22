using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Repositories.Friends;
using CC_Backend.Repositories.Stamps;
using CC_Backend.Repositories.User;
using CC_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Security.Claims;


namespace CC_Backend.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepo _userRepo;
        private readonly IEmailService _emailService;
        private readonly IFriendsRepo _friendsRepo;
        private readonly IStampsRepo _stampsRepo;
        private readonly ISearchUserService _searchUserService;
        public UserController(IUserRepo userRepo, IEmailService emailService, IFriendsRepo friendsRepo, IStampsRepo stampsRepo, UserManager<ApplicationUser> userManager, ISearchUserService searchUserService)

        {
            _userRepo = userRepo;
            _userManager = userManager;
            _emailService = emailService;
            _friendsRepo = friendsRepo;
            _stampsRepo = stampsRepo;
            _searchUserService = searchUserService;
        }

        [HttpGet]
        [Route("user/getallusers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepo.GetAllUsersAsync();
                var viewModelList = users.Select(user => new GetAllUsersViewModel
                {
                    DisplayName = user.DisplayName
                }).ToList();
                return Ok(viewModelList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("user/sendpasswordresettoken")]
        public async Task<IActionResult> SendPasswordResetToken([FromBody] SendPasswordResetTokenDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    return StatusCode(500, "Email not found!");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var (success, message) = await _emailService.SendEmailAsync(token, user.Email, user.UserName);

                if (!success)
                {
                    return StatusCode(500, message);
                }
                else
                {
                    return Ok(message);
                }

            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("user/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    return StatusCode(500, "Email not found!");
                }
                var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.newPassword);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/user/getuserprofile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var userProfile = await _userRepo.GetUserByIdAsync(userId);
                var friends = await _friendsRepo.GetFriendsAsync(userId);
                var stamps = await _stampsRepo.GetStampsFromUserAsync(userId);

                var viewModel = new GetUserProfileViewmodel
                {
                    DisplayName = userProfile.DisplayName,
                    ProfilePicture = userProfile.ProfilePicture,
                    StampsCollectedTotalCount = userProfile.StampsCollected.Count,
                    FriendsCount = friends.Count,
                    StampCollectedTotal = stamps,
                    Friends = friends
                };
                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/user/profilebydisplayname")]
        public async Task<IActionResult> GetUserProfileByDisplayname([FromBody] GetUserProfileByDisplaynameDTO dto)
        {
            try
            {
                var userProfile = await _userRepo.GetUserByDisplayNameAsync(dto.DisplayName);
                if (userProfile == null)
                {
                    return NotFound("User not found.");
                }
                var friends = await _friendsRepo.GetFriendsAsync(userProfile.Id);
                var stamps = await _stampsRepo.GetStampsFromUserAsync(userProfile.Id);

                var viewModel = new GetUserProfileViewmodel
                {
                    DisplayName = userProfile.DisplayName,
                    ProfilePicture = userProfile.ProfilePicture,
                    StampsCollectedTotalCount = userProfile.StampsCollected.Count,
                    FriendsCount = friends.Count,
                    StampCollectedTotal = stamps,
                    Friends = friends
                };
                return Ok(viewModel);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("user/search")]
        public async Task<IActionResult> SearchUser([FromQuery] string query)
        {
            try
            {
                var users = await _userRepo.SearchUserAsync(query);
                var result = _searchUserService.GetSearchUserViewModels(users, query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/user/feed")]
        public async Task<IActionResult> GetUserFeed()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var friends = await _friendsRepo.GetFriendsAsync(userId);
                var stampsCollectedByFriends = new List<UserFeedViewmodel>();
                

                foreach (var friend in friends)
                {

                    var profile = await _userRepo.GetUserByDisplayNameAsync(friend.DisplayName);
                    var stamps = await _stampsRepo.GetStampsCollectedFromUserAsync(profile.Id);
                    

                    foreach (var stamp in stamps)
                    {
                        var category = await _stampsRepo.GetCategoryFromStampAsync(stamp.Stamp.CategoryId);
                        var stampViewModel = new UserFeedViewmodel
                        {
                            DisplayName = profile.DisplayName,
                            StampCollectedId = stamp.StampCollectedId,
                            ProfilePicture = profile.ProfilePicture,
                            Category = category.Title,
                            StampIcon = stamp.Stamp.Icon,
                            StampName = stamp.Stamp.Name,
                            DateCollected = stamp.Geodata.DateWhenCollected // och har
                        };
                        stampsCollectedByFriends.Add(stampViewModel);
                    }
                }
                var orderedStamps = stampsCollectedByFriends.OrderByDescending(s => s.DateCollected);

                return Ok(orderedStamps);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
