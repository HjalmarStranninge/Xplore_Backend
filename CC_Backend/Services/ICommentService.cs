using CC_Backend.Models.DTOs;

namespace CC_Backend.Services
{
    public interface ICommentService
    {
        Task<bool> AddCommentAsync(CommentCreateDTO dto, string userId);
    }
}
