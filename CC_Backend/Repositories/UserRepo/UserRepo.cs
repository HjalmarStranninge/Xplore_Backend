using CC_Backend.Data;
using CC_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CC_Backend.Repositories.User
{
    public class UserRepo : IUserRepo
    {
        private readonly NatureAIContext _context;

        public UserRepo(NatureAIContext context)
        {
            _context = context;
        }

        // Get all users from the database
        public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync()
        {
            var result = await _context.Users.ToListAsync();
            return result;
        }


        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var result = await _context.Users.Include(x => x.StampsCollected).FirstOrDefaultAsync(x => x.Id == userId);

            return result;

        }

        
    }
}
