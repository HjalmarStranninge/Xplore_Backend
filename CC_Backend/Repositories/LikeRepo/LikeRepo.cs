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

        public async Task DeleteLikeAsync(Like like)
        {
            
            try
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Couldn't delete like");
            }
        }

        public async Task<Like> GetLikeByIdAsync(int likeId)
        {
            var result = await _context.Likes
                .Where(c => c.LikeId == likeId)
                .FirstOrDefaultAsync();
            return result;

        }

        public async Task<StampCollected> GetStampCollectedAsync(int stampCollectedId)
        {
            return await _context.StampsCollected
                .FirstOrDefaultAsync(sc => sc.StampCollectedId == stampCollectedId);
        }
    }
}
