using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Services
{
    public interface ICommentService
    {
        Task<bool> AddCommentAsync(CommentCreateDTO dto, string userId);
        Task<List<CommentViewModel>> ListCommentsFromStampCollected(int stampCollectedId);
    }
}
