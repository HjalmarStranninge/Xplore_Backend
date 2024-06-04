using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Repositories.UserRepo
{
    public interface IUserRepo
    {
        Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<IReadOnlyList<ApplicationUser>> SearchUserAsync(string displayName);
        Task<ApplicationUser> GetUserByDisplayNameAsync(string displayName);
        Task<bool> SetProfilePicAsync(string userId, byte[] profilePicture);
    }
}
