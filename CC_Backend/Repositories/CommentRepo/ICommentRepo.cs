using CC_Backend.Models;
public interface ICommentRepo
{
    Task<Comment> GetCommentByIdAsync(string userId, int stampcollectedId);
    Task AddCommentAsync(Comment comment);
    Task UpdateCommentAsync(Comment comment);
    Task DeleteCommentAsync(int commentId);
    Task<StampCollected> GetStampCollectedAsync(int stampCollectedId);
}