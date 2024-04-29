using Microsoft.EntityFrameworkCore;
using CC_Backend.Models;
namespace CC_Backend.Data
{
    public interface IDBRepo
    {
        Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync();
        Task<IReadOnlyList<StampCollected>> GetStampsFromUserAsync(string userId);
        Task<IReadOnlyList<ApplicationUser>> GetFriendsAsync(string userId);



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


        public async Task<IReadOnlyList<ApplicationUser>> GetFriendsAsync(string userId)
        {
            // Retrieve friends where the current user is FriendId1
            var friends1 = await _context.Friends
                .Where(f => f.FriendId1 == userId)
                .Select(f => f.FriendId2)
                .ToListAsync();

            // Retrieve friends where the current user is FriendId2
            var friends2 = await _context.Friends
                .Where(f => f.FriendId2 == userId)
                .Select(f => f.FriendId1)
                .ToListAsync();

            // Combine the two lists of friend IDs
            var friendIds = friends1.Concat(friends2).Distinct();

            // Now retrieve the user details for each friend ID
            var friends = await _context.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToListAsync();


            //In this code snippet:

           // - `friends1` is a list of user IDs where the current user is `FriendId1`.
           //- `friends2` is a list of user IDs where the current user is `FriendId2`.
           //- `friendIds` concatenates `friends1` and `friends2` and filters out duplicates using `Distinct()`.
           //-Finally, it retrieves the `AspNetUser` records for all the friend IDs.

            return friends;
        }

















    }
}
