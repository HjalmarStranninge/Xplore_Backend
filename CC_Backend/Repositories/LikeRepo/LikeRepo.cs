using CC_Backend.Data;
using CC_Backend.Models.DTOs;
using CC_Backend.Models;
using Org.BouncyCastle.Bcpg;
using Microsoft.EntityFrameworkCore;

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
            try
            {

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw new Exception("Couldn't add like.");
            }
        }

        public async Task DeleteLikeAsync(LikeDeleteDTO likeDeleteDto)
        {
            var like = await _context.Likes.FindAsync(likeDeleteDto.LikeId);
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Like was not found.");
            }
        }
        public async Task<StampCollected> GetStampCollectedAsync(int stampCollectedId)
        {
            return await _context.StampsCollected
                .FirstOrDefaultAsync(sc => sc.StampCollectedId == stampCollectedId);
        }
    }
}
