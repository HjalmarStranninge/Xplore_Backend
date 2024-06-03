using CC_Backend.Data;
using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;
using Microsoft.EntityFrameworkCore;

namespace CC_Backend.Repositories.UserRepo
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
            var result = await _context.Users
                .ToListAsync();

            return result;
        }

        // Get a user by id
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var result = await _context.Users
                .Include(x => x.StampsCollected)
                .FirstOrDefaultAsync(x => x.Id == userId);

            return result;

        }

        // Get a list of users through displayname containing search
        public async Task<List<ApplicationUser>> SearchUserAsync(string displayName)
        {
            var result = await _context.Users
                .Where(x => x.DisplayName
                .Contains(displayName))
                .ToListAsync();

            return result;
        }

        // Get a user by display name
        public async Task<ApplicationUser> GetUserByDisplayNameAsync(string displayName)
        {
            var result = await _context.Users
                .Include(x => x.StampsCollected)
                .FirstOrDefaultAsync(x => x.DisplayName == displayName);

            return result;
        }

        // Set a new profile picture
        public async Task<bool> SetProfilePicAsync(string userId, byte[] profilePicture)
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

        //// Gets a list of viewmodels for searched users
        //public List<SearchUserViewModel> GetSearchUserViewModels(List<ApplicationUser> users, string query)
        //{
        //    return users
        //        .Select(u => new SearchUserViewModel
        //        {
        //            DisplayName = u.DisplayName,
        //            ProfilePicture = u.ProfilePicture
        //        })
        //        .Where(u => u.DisplayName.Contains(query))
        //        .Take(5)
        //        .ToList();
        //}
    }
}
