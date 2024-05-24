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

    // Fetch a specific comment by its Id
    public async Task<Comment> GetCommentByIdAsync(string userId, int stampCollectedId)
    {
        var result = await _context.Comments
            .Where(c => c.UserId == userId && c.StampCollectedId == stampCollectedId)
            .FirstOrDefaultAsync();
        return result;

    }

    // Save a new comment to the database
    public async Task AddCommentAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    // Update an existing comment in the database
    public async Task UpdateCommentAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    // Delete a comment from the database
    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    // Get all the comments on a collected stamp.
    public async Task<ICollection<CommentViewModel>> GetCommentsFromStampCollectedAsync(int stampCollectedId)
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