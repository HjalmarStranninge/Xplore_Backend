using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Repositories.CommentRepo;
using CC_Backend.Repositories.FriendsRepo;
using CC_Backend.Repositories.LikeRepo;
using CC_Backend.Repositories.StampsRepo;
using CC_Backend.Repositories.UserRepo;
using CC_Backend.Services;
using CC_Backend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        private readonly IUserService _userService;
        private readonly IStampService _stampService;

        public UserController(IUserRepo userRepo, IFriendsRepo friendsRepo, IStampsRepo stampsRepo, 
            UserManager<ApplicationUser> userManager, ICommentRepo commentRepo, ILikeRepo likeRepo, IUserService userService, IStampService stampService)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _friendsRepo = friendsRepo;
            _stampsRepo = stampsRepo;
            _commentRepo = commentRepo;
            _likeRepo = likeRepo;
            _userService = userService;
            _stampService = stampService;
        }

        // Get all users
        [HttpGet("getallusers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                //var users = await _userRepo.GetAllUsersAsync();
                var result = _userService.CreateAllUsersViewModels();
                return Ok(result);
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
                var userId = UserUtilities.ExtractUserIdFromToken(User);

                var friends = await _userService.CreateFriendViewModels(userId);
                var stamps = await _stampService.CreateStampViewModelsOfUser(userId);

                var userProfileViewModel = await _userService.CreateUserProfileViewModelById(userId, friends, stamps);

                return Ok(userProfileViewModel);
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
                var userId = await _userRepo.GetUserByDisplayNameAsync(dto.DisplayName);
                if (userId == null)
                {
                    return NotFound("User not found.");
                }

                var friends = await _userService.CreateFriendViewModels(userId.Id);
                var stamps = await _stampService.CreateStampViewModelsOfUser(userId.Id);

                var user = await _userService.CreateUserProfileViewModelByName(userId,friends,stamps);
                return Ok(user);
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
                var users = await _userRepo.GetAllUsersAsync();
                var result = _userService.CreateSearchUserViewModels(users, query);
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
                var userId = UserUtilities.ExtractUserIdFromToken(User);

                var userFeed = await _userService.CreateUserFeed(userId);

                return Ok(userFeed);

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
                var userId = UserUtilities.ExtractUserIdFromToken(User);

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
