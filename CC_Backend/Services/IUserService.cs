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

        List<AllUsersViewModel> CreateAllUsersViewModels(IEnumerable<ApplicationUser> users);

        List<FriendViewModel> CreateFriendViewModels(IEnumerable<ApplicationUser> friends);

        UserProfileViewmodel CreateUserProfileViewModel(ApplicationUser user, IReadOnlyList<Friends> friends, ICollection<StampViewModel> stamps, List<FriendViewModel> friendsViewModels);

        List<SearchUserViewModel> GetSearchUserViewModels(List<ApplicationUser> users, string query);
    }
}
