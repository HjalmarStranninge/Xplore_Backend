using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Repositories.Friends
{
    public interface IFriendsRepo
    {
        Task<IReadOnlyList<FriendViewModel>> GetFriendsAsync(string userId);
        Task<(bool success, string message)> AddFriendAsync(string userId, string friendDisplayName);
        Task<(bool success, string message)> RemoveFriendAsync(string userId, string friendDisplayName);
    }
}
