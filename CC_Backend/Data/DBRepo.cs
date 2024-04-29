using Microsoft.EntityFrameworkCore;
using CC_Backend.Models;
namespace CC_Backend.Data
{
    public interface IDBRepo
    {
        Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync();
        Task<IReadOnlyList<StampCollected>> GetStampsFromUserAsync(string userId);



    }



    public class DBRepo: IDBRepo
    {
        private readonly NatureAIContext _context;

        public DBRepo(NatureAIContext context)
        {
            _context = context;
        }

        
        // Get all users from the database
        public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync()
        {
            var result = await _context.Users.ToListAsync();
            return result;
        }


        // Get all stamps from user

        public async Task<IReadOnlyList<StampCollected>> GetStampsFromUserAsync(string userId)
        {
            var result = await _context.Users
                .Include(u => u.StampsCollected)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.StampsCollected)
                .ToListAsync();
            
            return result;
        }


        














    }
}
