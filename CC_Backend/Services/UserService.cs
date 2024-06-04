using System.Security.Claims;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Repositories.FriendsRepo;
using CC_Backend.Repositories.UserRepo;
using CC_Backend.Repositories.StampsRepo;
using CC_Backend.Repositories.CommentRepo;
using CC_Backend.Repositories.LikeRepo;
using CC_Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace CC_Backend.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IFriendsRepo _friendsRepo;
        private readonly IUserRepo _userRepo;
        private readonly IStampsRepo _stampsRepo;
        private readonly ICommentRepo _commentRepo;
        private readonly ILikeRepo _likeRepo;
        private readonly ICommentService _commentService;

        public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService, IFriendsRepo friendsRepo, IUserRepo userRepo, IStampsRepo stampsRepo, ICommentRepo commentRepo, ILikeRepo likeRepo, ICommentService commentService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _friendsRepo = friendsRepo;
            _userRepo = userRepo;
            _stampsRepo = stampsRepo;
            _commentRepo = commentRepo;
            _likeRepo = likeRepo;
            _commentService = commentService;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.Email,
            };
            return await _userManager.CreateAsync(user, dto.Password);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description="Email not found"});
            }
            return await _userManager.ResetPasswordAsync(user, dto.Token, dto.newPassword);
        }

        public async Task<(bool success, string message)> SendPasswordResetTokenAsync(SendPasswordResetTokenDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return (false, "Email not found.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _emailService.SendEmailAsync(token, user.Email, user.UserName);
        }

        public async Task<List<AllUsersViewModel>> CreateAllUsersViewModels()
        {
            var users = await _userRepo.GetAllUsersAsync();
            var allUsersViewModels = users.Select(user => new AllUsersViewModel
            {
                DisplayName = user.DisplayName,
            })
            .ToList();
            return allUsersViewModels;
        }

        public async Task<List<FriendViewModel>> CreateFriendViewModels(string userId)
        {
            var friendsOfUser = await _friendsRepo.GetFriendsAsync(userId);
            var userFriends = new List<FriendViewModel>();

            foreach (var friend in friendsOfUser)
            {
                var viewModel = new FriendViewModel
                {
                    UserId = friend.Id,
                    DisplayName = friend.DisplayName,
                    ProfilePicture = friend.ProfilePicture
                };
                userFriends.Add(viewModel);
            }
            return userFriends;
        }

        public async Task<UserProfileViewmodel> CreateUserProfileViewModelByName(ApplicationUser user, IReadOnlyList<FriendViewModel> friends, ICollection<StampViewModel> stamps)
        {
            var userProfileModel = new UserProfileViewmodel
            {
                DisplayName = user.DisplayName,
                ProfilePicture = user.ProfilePicture,
                StampsCollectedTotalCount = user.StampsCollected.Count,
                FriendsCount = friends.Count,
                StampCollectedTotal = stamps,
                Friends = await CreateFriendViewModels(user.Id),
            };

            return userProfileModel;
        }
        public async Task<UserProfileViewmodel> CreateUserProfileViewModelById(string userId, IReadOnlyList<FriendViewModel> friends, ICollection<StampViewModel> stamps)
        {
            var userById = await _userRepo.GetUserByIdAsync(userId);

            var userProfileModel = new UserProfileViewmodel
            {
                DisplayName = userById.DisplayName,
                ProfilePicture = userById.ProfilePicture,
                StampsCollectedTotalCount = userById.StampsCollected.Count,
                FriendsCount = friends.Count,
                StampCollectedTotal = stamps,
                Friends = await CreateFriendViewModels(userId),
            };

            return userProfileModel;
        }

        public async Task<List<SearchUserViewModel>> CreateSearchUserViewModels(IReadOnlyList<ApplicationUser> users, string query)
        {
            var userSearch = await _userRepo.SearchUserAsync(query);

            var searchLimit = 5;
            var matchingUsers = new List<SearchUserViewModel>();

            foreach (var user in userSearch)
            {
                var searchUserViewModel = new SearchUserViewModel
                {
                    DisplayName = user.DisplayName,
                    ProfilePicture = user.ProfilePicture
                };

                matchingUsers.Add(searchUserViewModel);

                if (matchingUsers.Count == searchLimit)
                {
                    break;
                }
            }
            return matchingUsers;
        }

        public IReadOnlyList<FriendViewModel> ConvertToFriendViewModels(IReadOnlyList<ApplicationUser> users)
        {
            var friendViewModels = users.Select(user => new FriendViewModel
            {
                UserId = user.Id,
                DisplayName = user.DisplayName,
                ProfilePicture = user.ProfilePicture
            }).ToList();

            return friendViewModels;
        }

        public async Task<List<UserFeedViewmodel>> CreateUserFeed(string userId)
        {
            var friends = await _friendsRepo.GetFriendsAsync(userId);
            var stampsCollectedByFriends = new List<UserFeedViewmodel>();

            foreach (var friend in friends)
            {
                var profile = await _userRepo.GetUserByIdAsync(friend.Id);
                var stamps = await _stampsRepo.GetStampsCollectedFromUserAsync(profile.Id);

                foreach (var stamp in stamps)
                {
                    var category = await _stampsRepo.GetCategoryFromStampAsync(stamp.Stamp.CategoryId);
                    var comments = await _commentService.ListCommentsFromStampCollected(stamp.StampCollectedId);
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

            return stampsCollectedByFriends.OrderByDescending(s => s.DateCollected).ToList();
        }

    }
}
