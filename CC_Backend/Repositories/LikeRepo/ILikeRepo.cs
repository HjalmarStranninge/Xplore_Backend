using CC_Backend.Models;

namespace CC_Backend.Repositories.LikeRepo
{
    public interface ILikeRepo
    {
        Task AddLikeAsync(Like like);
        Task DeleteLikeAsync(int likeId);
        Task<StampCollected> GetStampCollectedAsync(int stampCollectedId);
    }
}
