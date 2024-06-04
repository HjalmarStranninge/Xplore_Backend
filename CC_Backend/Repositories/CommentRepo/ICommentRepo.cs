using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Repositories.CommentRepo
{
    public interface ICommentRepo
    {
        Task<Comment> GetCommentByIdAsync(string userId, int stampCollectedId);
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int commentId);
        Task<ICollection<Comment>> GetCommentsFromStampCollectedAsync(int stampCollectedId);
    }
}
