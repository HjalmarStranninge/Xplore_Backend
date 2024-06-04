using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;

namespace CC_Backend.Services
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDTO dto);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDTO dto);

        Task<(bool success, string message)> SendPasswordResetTokenAsync(SendPasswordResetTokenDto dto);

        Task<List<AllUsersViewModel>> CreateAllUsersViewModels();

        Task<List<FriendViewModel>> CreateFriendViewModels(string userId);

        Task<UserProfileViewmodel> CreateUserProfileViewModelByName(ApplicationUser user, IReadOnlyList<FriendViewModel> friends, ICollection<StampViewModel> stamps);
        Task<UserProfileViewmodel> CreateUserProfileViewModelById(string userId, IReadOnlyList<FriendViewModel> friends, ICollection<StampViewModel> stamps);
        Task<List<SearchUserViewModel>> CreateSearchUserViewModels(IReadOnlyList<ApplicationUser> users, string query);
        Task<List<UserFeedViewmodel>> CreateUserFeed(string userId);
    }
}
