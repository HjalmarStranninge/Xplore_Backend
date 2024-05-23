using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;
public interface ICommentRepo
{
    Task<Comment> GetCommentByIdAsync(string userId, int stampCollectedId);
    Task AddCommentAsync(Comment comment);
    Task UpdateCommentAsync(Comment comment);
    Task DeleteCommentAsync(int commentId);
    Task<StampCollected> GetStampCollectedAsync(int stampCollectedId);
    Task<ICollection<CommentViewModel>> GetCommentFromStampCollected(int stampCollectedId);
}