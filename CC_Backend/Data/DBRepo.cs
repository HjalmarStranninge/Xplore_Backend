using Microsoft.EntityFrameworkCore;
using CC_Backend.Models;
namespace CC_Backend.Data
{
    public class DBRepo
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


        











    }
}
