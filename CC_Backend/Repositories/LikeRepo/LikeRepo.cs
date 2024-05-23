using CC_Backend.Data;
using CC_Backend.Models;
using Org.BouncyCastle.Bcpg;

namespace CC_Backend.Repositories.LikeRepo
{
    public class LikeRepo : ILikeRepo

    {
        private readonly NatureAIContext _context;

        public LikeRepo(NatureAIContext context)
        {
            _context = context;
        }
        public async Task AddLikeAsync(Like like)
        {
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLikeAsync(int likeId)
        {
            var like = await _context.Likes.FindAsync(likeId);
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }

        public Task<StampCollected> GetStampCollectedAsync(int stampCollectedId)
        {
            throw new NotImplementedException();
        }
    }
}
