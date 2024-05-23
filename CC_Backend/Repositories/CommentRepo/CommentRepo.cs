using CC_Backend.Data;
using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CommentRepo : ICommentRepo
{
    private readonly NatureAIContext _context;

    public CommentRepo(NatureAIContext context)
    {
        _context = context;
    }

    public async Task<Comment> GetCommentByIdAsync(string userId, int stampCollectedId) 
    {
        var result = await _context.Comments
            .Where(c => c.UserId == userId && c.StampCollectedId == stampCollectedId)
            .FirstOrDefaultAsync();
        return result;

    }

    public async Task AddCommentAsync(Comment comment) // Byt till DTO
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(Comment comment) // Byt till DTO
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int commentId) 
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<StampCollected> GetStampCollectedAsync(int stampCollectedId) 
    {
        return await _context.StampsCollected
            .FirstOrDefaultAsync(sc => sc.StampCollectedId == stampCollectedId);
    }

    public async Task<ICollection<CommentViewModel>> GetCommentFromStampCollected(int stampCollectedId)
    {
        var commentsList = await _context.Comments
            .Where(c => c.StampCollectedId == stampCollectedId)
            .Include(u => u.User)
            .ToListAsync();

        var comments = new List<CommentViewModel>();
        foreach (var comment in commentsList)
        {
            var commentViewModel = new CommentViewModel
            {
                CommenterDisplayName = comment.User.DisplayName,
                CommenterProfilePic = comment.User.ProfilePicture,
                CommentContent = comment.Content,
                PostedAt = comment.CreatedAt,
            };

            comments.Add(commentViewModel);
        }
        return comments;
             
    }
}