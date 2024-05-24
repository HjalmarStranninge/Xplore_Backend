using CC_Backend.Data;
using CC_Backend.Models.DTOs;
using CC_Backend.Models;
using Org.BouncyCastle.Bcpg;
using Microsoft.EntityFrameworkCore;
using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Repositories.LikeRepo
{
    public class LikeRepo : ILikeRepo
    {
        private readonly NatureAIContext _context;

        public LikeRepo(NatureAIContext context)
        {
            _context = context;
        }

        // Saves a like to the database
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

        // Deletes a like from the database
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

        // Gets all the likes of a specific collected stamp
        public async Task<ICollection<LikeViewModel>> GetLikesFromStampCollected(int stampCollectedId)
        {
            var likesList = await _context.Likes
                .Where(c => c.StampCollectedId == stampCollectedId)
                .Include(u => u.User)
                .ToListAsync();

            var likes = new List<LikeViewModel>();

            foreach (var like in likesList)
            {
                var LikeViewModel = new LikeViewModel
                {
                    LikeId = like.LikeId,
                    StampCollectedId = like.LikeId,
                    UserId = like.UserId
                };

                likes.Add(LikeViewModel);
            }
            return likes;

        }

        // Fetch a like from its Id
        public async Task<Like> GetLikeByIdAsync(int likeId)
        {
            var result = await _context.Likes
                .Where(c => c.LikeId == likeId)
                .FirstOrDefaultAsync();

            return result;
        }
    }
}
