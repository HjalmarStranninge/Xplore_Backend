using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Repositories.Friends
{
    public interface IFriendsRepo
    {
        Task<IReadOnlyList<FriendViewModel>> GetFriendsAsync(string userId);
        Task<(bool success, string message)> AddFriendAsync(string userId, AddFriendDTO dto);
        Task<(bool success, string message)> RemoveFriendAsync(string userId, RemoveFriendDTO dto);
    }
}
