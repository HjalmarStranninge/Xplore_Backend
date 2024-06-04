using CC_Backend.Data;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Models;
using Microsoft.EntityFrameworkCore;
using CC_Backend.Models.DTOs;
using CC_Backend.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CC_Backend.Repositories.StampsRepo
{
    public class StampsRepo : IStampsRepo
    {
        private readonly NatureAIContext _context;

        public StampsRepo(NatureAIContext context)
        {
            _context = context;
        }

        // Get all collected stamps from user
        public async Task<ICollection<StampCollected>> GetStampsFromUserAsync(string userId)
        {
            var result = await _context.Users
                .Include(u => u.StampsCollected)
                    .ThenInclude(u => u.Stamp)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.StampsCollected)
                .ToListAsync();

            return result;
        }

        // Get all stampscollected and geodata from user
        public async Task<ICollection<StampCollected>> GetStampsCollectedFromUserAsync(string userId)
        {
            var result = await _context.Users
                .Include(u => u.StampsCollected)
                    .ThenInclude(u => u.Geodata)
                    .Include(u => u.StampsCollected)
                    .ThenInclude(u => u.Stamp)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.StampsCollected)
                .ToListAsync();

            return result;
        }

        // Save a StampCollected-object to the database it and connect a user to it.
        public async Task AwardStampToUserAsync(string userId, StampCollected stamp)
        {
            try
            {
                _context.StampsCollected.Add(stamp);
                await _context.SaveChangesAsync();

                var user = await _context.Users
                    .Include(u => u.StampsCollected)
                    .SingleOrDefaultAsync(u => u.Id == userId);

                if (user.StampsCollected == null)
                {
                    user.StampsCollected = new List<StampCollected>();
                }

                user.StampsCollected.Add(stamp);

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add stamp.", ex);
            }
        }

        // Get information about a selected stamp
        public async Task<Stamp> GetSelectedStampAsync(int stampId)
        {
            try
            {
                var stamp = await _context.Stamps
                    .Include(s => s.Category)
                    .SingleOrDefaultAsync(s => s.StampId == stampId);

                return stamp;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve selected stamp information", ex);
            }
        }

        // Get all stamps in a category
        public async Task<Category> GetStampsFromCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories
                .Include(c => c.Stamps)
                .SingleOrDefaultAsync(c => c.CategoryId == categoryId);

                if (category == null)
                {
                    return (null);
                }

                return (category);
            }
            catch (Exception ex)
            {
                throw new Exception($"Something went wrong. {ex}");
            }
        }

        // Get which category a stamp belongs to
        public async Task<CategoryViewModel> GetCategoryFromStampAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories
                .SingleOrDefaultAsync(c => c.CategoryId == categoryId);

                if (category == null)
                {
                    return null;
                }

                var categoryViewModel = new CategoryViewModel
                {
                    Id = category.CategoryId,
                    Title = category.Title
                };

                return categoryViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        // Get all stamps you have collected in a category 
        public async Task<List<CategoryWithStampsCountDTO>> GetCategoryStampCountsAsync()
        {
            var allStamps = await _context.Stamps.Include(s => s.Category).ToListAsync();
            var collectedStamps = await _context.StampsCollected.Include(sc => sc.Stamp).ThenInclude(s => s.Category).ToListAsync();

            var categoryStampCounts = allStamps
                .GroupBy(s => s.Category.Title)
                .Select(g => new CategoryWithStampsCountDTO
                {
                    Title = g.Key,
                    TotalStamps = g.Count(),
                    CollectedStamps = collectedStamps.Count(cs => cs.Stamp.Category.Title == g.Key)
                })
                .ToList();

            return categoryStampCounts;
        }

        // Create a new stamp
        public async Task<bool> CreateStampAsync(Stamp stamp)
        {
            try
            {
                _context.Stamps.Add(stamp);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("Unable to create stamp.", ex);
            }
        }

        // Add a stamp to a category
        public async Task AddStampToCategoryAsync(Stamp stamp, string categoryTitle)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Title == categoryTitle);

                if (category == null)
                {
                    throw new Exception("Category not found.");
                }

                category.Stamps.Add(stamp);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add stamp to category.", ex);
            }
        }

        // Get a category with all the stamps in it
        public async Task<Category> FindCategoryWithStampAsync(string categoryTitle)
        {
            try
            {
                return await _context.Categories
                .Include(c => c.Stamps)
                .FirstOrDefaultAsync(c => c.Title == categoryTitle);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to find category with stamp.", ex);
            }
        }

        // Fetch a specific collected stamp by its Id
        public async Task<StampCollected> GetStampCollectedAsync(int stampCollectedId)
        {
            return await _context.StampsCollected
                .FirstOrDefaultAsync(sc => sc.StampCollectedId == stampCollectedId);
        }
    }
}
