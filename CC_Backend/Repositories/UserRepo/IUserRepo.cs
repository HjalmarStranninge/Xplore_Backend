using CC_Backend.Models;

namespace CC_Backend.Repositories.User
{
    public interface IUserRepo
    {
        Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> GetUserByDisplayNameAsync(string displayName);
        Task<bool> SetUserProfile(string userId, byte[] profilePicture);
    }
}
