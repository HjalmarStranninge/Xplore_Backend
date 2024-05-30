using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Repositories.CommentRepo;
using CC_Backend.Repositories.FriendsRepo;
using CC_Backend.Repositories.LikeRepo;
using CC_Backend.Repositories.StampsRepo;
using CC_Backend.Repositories.UserRepo;
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
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepo _userRepo;     
        private readonly IFriendsRepo _friendsRepo;
        private readonly IStampsRepo _stampsRepo;
        private readonly ICommentRepo _commentRepo;
        private readonly ILikeRepo _likeRepo;
        private readonly ISearchUserService _searchUserService;
        private readonly IUserService _userService;

        public UserController(IUserRepo userRepo, IFriendsRepo friendsRepo, IStampsRepo stampsRepo, 
            UserManager<ApplicationUser> userManager, ICommentRepo commentRepo, ILikeRepo likeRepo, ISearchUserService searchUserService, IUserService userService)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _friendsRepo = friendsRepo;
            _stampsRepo = stampsRepo;
            _commentRepo = commentRepo;
            _likeRepo = likeRepo;
            _searchUserService = searchUserService;
            _userService = userService;
        }

        // Get all users
        [HttpGet("getallusers")]
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

        // Get all information about a user
        [HttpGet("getuserprofile")]
        [Authorize]
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

        // Get a user by displayname
        [HttpPost("profilebydisplayname")]
        [Authorize]
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

        // Gets searched for users from database
        [HttpGet("searchuser")]
        [Authorize]
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

        // View users friends and their activity
        [HttpGet("feed")]
        [Authorize]
        public async Task<IActionResult> GetUserFeed()
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var friends = await _friendsRepo.GetFriendsAsync(userId);
                var stampsCollectedByFriends = new List<UserFeedViewmodel>();
                

                foreach (var friend in friends)
                {

                    var profile = await _userRepo.GetUserByIdAsync(friend.UserId);
                    var stamps = await _stampsRepo.GetStampsCollectedFromUserAsync(profile.Id);
                    

                    foreach (var stamp in stamps)
                    {
                        var category = await _stampsRepo.GetCategoryFromStampAsync(stamp.Stamp.CategoryId);
                        var comments = await _commentRepo.GetCommentsFromStampCollectedAsync(stamp.StampCollectedId);
                        var likes = await _likeRepo.GetLikesFromStampCollected(stamp.StampCollectedId);

                        var stampViewModel = new UserFeedViewmodel
                        {
                            DisplayName = profile.DisplayName,
                            StampCollectedId = stamp.StampCollectedId,
                            ProfilePicture = profile.ProfilePicture,
                            Category = category.Title,
                            StampIcon = stamp.Stamp.Icon,
                            StampName = stamp.Stamp.Name,
                            DateCollected = stamp.Geodata.DateWhenCollected,
                            Comments = comments,
                            LikeCount = likes.Count
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

        // Set a profile picture to a user
        [HttpPost("setprofilepicture")]
        [Authorize]
        public async Task<IActionResult> SetProfilePicture([FromBody] SetProfilePictureDTO dto)
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                bool result = await _userRepo.SetProfilePicAsync(userId, dto.ProfilePicture);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
