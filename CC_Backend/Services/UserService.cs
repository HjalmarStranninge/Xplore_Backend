using System.Security.Claims;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace CC_Backend.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
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

        public List<AllUsersViewModel> CreateAllUsersViewModels(IEnumerable<ApplicationUser> users)
        {
            var allUsersViewModels = users.Select(user => new AllUsersViewModel
            {
                DisplayName = user.DisplayName,
            }).ToList();
            return allUsersViewModels;
        }

        public List<FriendViewModel> CreateFriendViewModels(IEnumerable<ApplicationUser> friends)
        {
            var userFriends = new List<FriendViewModel>();

            foreach (var friend in friends)
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

        public UserProfileViewmodel CreateUserProfileViewModel(ApplicationUser user, IReadOnlyList<Friends> friends, ICollection<StampViewModel> stamps, List<FriendViewModel> friendsViewModels)
        {
            var userProfileModel = new UserProfileViewmodel
            {
                DisplayName = user.DisplayName,
                ProfilePicture = user.ProfilePicture,
                StampsCollectedTotalCount = user.StampsCollected.Count,
                FriendsCount = friends.Count,
                StampCollectedTotal = stamps,
                Friends = friendsViewModels,
            };

            return userProfileModel;
        }

        public List<SearchUserViewModel> GetSearchUserViewModels(List<ApplicationUser> users, string query)
        {
            return users
                .Select(u => new SearchUserViewModel
                {
                    DisplayName = u.DisplayName,
                    ProfilePicture = u.ProfilePicture
                })
                .Where(u => u.DisplayName.Contains(query))
                .Take(5)
                .ToList();
        }

        //public List<UserProfileViewmodel> CreateUserProfileViewmodel(ApplicationUser user, IEnumerable)
        //{
        //    var userProfileModel = new UserProfileViewmodel
        //    {
        //        DisplayName = userProfile.DisplayName,
        //        ProfilePicture = userProfile.ProfilePicture,
        //        StampsCollectedTotalCount = userProfile.StampsCollected.Count,
        //        FriendsCount = friends.Count,
        //        StampCollectedTotal = stamps,
        //        Friends = friends
        //    };
        //    return userProfileModel;
        //}

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

    }
}
