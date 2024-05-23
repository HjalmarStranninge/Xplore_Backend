using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Repositories.LikeRepo
{
    public interface ILikeRepo
    {
        Task AddLikeAsync(Like like);
        Task DeleteLikeAsync(Like like);
        Task<StampCollected> GetStampCollectedAsync(int stampCollectedId);
        Task<Like> GetLikeByIdAsync(int likeId);
        Task<ICollection<LikeViewModel>> GetLikesFromStampCollected(int stampCollectedId);

    }
}
