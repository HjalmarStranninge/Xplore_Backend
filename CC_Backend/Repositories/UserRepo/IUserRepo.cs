using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Repositories.User
{
    public interface IUserRepo
    {
        Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<List<ApplicationUser>> SearchUserAsync(string displayName);
        Task<ApplicationUser> GetUserByDisplayNameAsync(string displayName);
        Task<bool> SetUserProfile(string userId, byte[] profilePicture);
    }
}
