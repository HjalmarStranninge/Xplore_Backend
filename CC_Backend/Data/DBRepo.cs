using Microsoft.EntityFrameworkCore;
using CC_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using CC_Backend.Models.Viewmodels;
namespace CC_Backend.Data
{
    public interface IDBRepo
    {
        Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync();
        Task<ICollection<StampViewModel>> GetStampsFromUserAsync(string userId);
        Task<IReadOnlyList<FriendViewModel>> GetFriendsAsync(string userId);
        Task<(bool success, string message)> AddFriendAsync(string userId, string friendUserName);
        Task<(bool success, string message)> RemoveFriendAsync(string userId, string friendUserName);
        Task AwardStampToUserAsync(string userId, StampCollected stamp);
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
                    
                };
                stampsList.Add(stampViewModel);
            }
            
            return stampsList;
        }


        public async Task<IReadOnlyList<FriendViewModel>> GetFriendsAsync(string userId)
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
            var friendsResult = await _context.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToListAsync();


            // Create a list of viewmodels and convert each friend object to a viewmodel and put it into the list.
            var friends = new List<FriendViewModel>();

            foreach(var friend in friendsResult)
            {
                var viewModel = new FriendViewModel
                {
                    UserName = friend.UserName
                };
                friends.Add(viewModel);              
            }


            //In this code snippet:

           // - `friends1` is a list of user IDs where the current user is `FriendId1`.
           //- `friends2` is a list of user IDs where the current user is `FriendId2`.
           //- `friendIds` concatenates `friends1` and `friends2` and filters out duplicates using `Distinct()`.
           //-Finally, it retrieves the `AspNetUser` records for all the friend IDs.

            return friends;
        }

        // Adds a new friend by getting the corresponding user id of the username that the logged in user is trying to add.
        public async Task<(bool success, string message)> AddFriendAsync(string userId, string friendUserName)
        {
            try
            {
                var friendToAddId = await _context.Users
                    .Where(u => u.UserName == friendUserName)
                    .Select(u => u.Id)
                    .SingleOrDefaultAsync();

                // Check if the user is trying to add themselves as a friend.
                if (userId == friendToAddId)
                {
                    return (false, "You cannot add yourself as a friend.");
                }

                // Check if the users are already friends.
                if ((_context.Friends.Any(f => f.FriendId1 == userId && f.FriendId2 == friendToAddId)) ||
                    (_context.Friends.Any(f => f.FriendId1 == friendToAddId && f.FriendId2 == userId)))
                {
                    return (false, "Users are already friends.");
                }


                else
                {
                    var newFriend = new Friends
                    {
                        FriendId1 = userId,
                        FriendId2 = friendToAddId
                    };

                    _context.Friends.Add(newFriend);
                    await _context.SaveChangesAsync();

                    return (true, "Friend added successfully.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Unable to add friend: {ex.Message}");
            }
        }

        // Remove a friend from a users friendlist.
        public async Task<(bool success, string message)> RemoveFriendAsync(string userId, string friendUserName)
        {
            try
            {
                var friendToDeleteId = await _context.Users
                    .Where(u => u.UserName == friendUserName)
                    .Select(u => u.Id)
                    .SingleOrDefaultAsync();

                var friendship = await _context.Friends
                    .Where (f => f.FriendId1 == userId && f.FriendId2 == friendToDeleteId || f.FriendId2 == userId && f.FriendId1 == friendToDeleteId)
                    .SingleOrDefaultAsync();

                if (friendship != null)
                {
                    _context.Friends.Remove(friendship);
                    await _context.SaveChangesAsync();
                    return (true, "Friend removed.");
                }

                else
                {
                    return (false, "Friend not found.");
                }                
                
            }
            catch (Exception ex)
            {

                return (false, $"Unable to delete friend: {ex.Message}");
            }
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
    }
}
