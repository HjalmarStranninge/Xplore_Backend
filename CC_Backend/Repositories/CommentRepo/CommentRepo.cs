using CC_Backend.Data;
using CC_Backend.Models;
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

    public async Task<Comment> GetCommentByIdAsync(string userId, int stampcollectedId)
    {
        var result = await _context.Comments
            .Where(c => c.UserId == userId && c.StampCollectedId == stampcollectedId)
            .FirstOrDefaultAsync();
        return result;

    }

    public async Task AddCommentAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(Comment comment)
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
}