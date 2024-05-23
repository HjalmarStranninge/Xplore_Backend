using CC_Backend.Models;
using CC_Backend.Models.DTOs;

namespace CC_Backend.Repositories.LikeRepo
{
    public interface ILikeRepo
    {
        Task AddLikeAsync(Like like);
        Task DeleteLikeAsync(Like like);
        Task<StampCollected> GetStampCollectedAsync(int stampCollectedId);
        Task<Like> GetLikeByIdAsync(int likeId);

    }
}
