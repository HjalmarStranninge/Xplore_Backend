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

        // Get a user by id
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var result = await _context.Users.Include(x => x.StampsCollected).FirstOrDefaultAsync(x => x.Id == userId);

            return result;

        }

        // Get a by display name
        public async Task<ApplicationUser> GetUserByDisplayNameAsync(string displayName)
        {
            var result = await _context.Users.Include(x => x.StampsCollected).FirstOrDefaultAsync(x => x.DisplayName == displayName);

            return result;
        }

        // Set a profil pic to the user
        public async Task<bool> SetUserProfile(string userId, byte[] profilePicture)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                user.ProfilePicture = profilePicture;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
