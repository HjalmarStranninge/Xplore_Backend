using CC_Backend.Data;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Models;
using Microsoft.EntityFrameworkCore;
using CC_Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CC_Backend.Repositories.Stamps
{
    public class StampsRepo : IStampsRepo
    {
        private readonly NatureAIContext _context;

        public StampsRepo(NatureAIContext context)
        {
            _context = context;
        }

        // Get all stamps from user

        public async Task<ICollection<StampViewModel>> GetStampsFromUserAsync(string userId)
        {
            var result = await _context.Users
                .Include(u => u.StampsCollected)
                    .ThenInclude(u => u.Stamp)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.StampsCollected)
                .ToListAsync();


            // Create a list of viewmodels that mirrors the list of stamps that a user has collected.
            var stampsList = new List<StampViewModel>();

            foreach (var stamp in result)
            {
                var stampViewModel = new StampViewModel
                {
                    Name = stamp.Stamp.Name,
                    Icon = stamp.Stamp.Icon

                };
                stampsList.Add(stampViewModel);
            }

            return stampsList;
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

                // Add the new StampCollected object to the StampsCollected collection.
                if (user.StampsCollected == null)
                {
                    // Initialize the collection if necessary.
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
        public async Task<StampDTO> GetSelectedStamp(int stampId)
        {
            try
            {
                var stamp = await _context.Stamps
                    .Include(s => s.Category)
                    .SingleOrDefaultAsync(s => s.StampId == stampId);

                var stampDto = new StampDTO
                {
                    Name = stamp.Name,
                    Facts = stamp.Facts,
                    Rarity = stamp.Rarity,
                    Icon = stamp.Icon,
                    Latitude = stamp.Latitude,
                    Longitude = stamp.Longitude,
                    Category = new CategoryDTO
                    {
                        Title = stamp.Category?.Title ?? ""
                    }
                };

                return stampDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve selected stamp information", ex);
            }
        }

        // Get all stamps in a category
        public async Task<(CategoryDTO?, string)> GetStampsFromCategory(int categoryId)
        {
            try
            {
                var category = await _context.Categories
                .Include(c => c.Stamps)
                .SingleOrDefaultAsync(c => c.CategoryId == categoryId);

                if (category == null)
                {
                    return (null, "Category not found.");
                }

                var categoryDto = new CategoryDTO
                {
                    Title = category.Title,
                    Stamps = category.Stamps.Select(stamps => new StampDTO
                    {
                        Name = stamps.Name,
                        Facts = stamps.Facts,
                        Rarity = stamps.Rarity,
                        Icon = stamps.Icon,
                        Latitude = stamps.Latitude,
                        Longitude = stamps.Longitude,
                    }).ToList()
                };
                return (categoryDto, "Success");
            }
            catch (Exception ex)
            {
                return (null,$"Something went wrong. {ex}");
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

        public async Task<Category> FindCategoryWithStampAsync(string categoryTitle)
        {
            try
            {
                return await _context.Categories
                .Include(c => c.Stamps)
                .FirstOrDefaultAsync(c => c.Title == categoryTitle);
            }catch (Exception ex)
            {
                throw new Exception("Unable to find category with stamp.", ex);
            }
        }

    }
}
