using CC_Backend.Data;
using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;
using Microsoft.EntityFrameworkCore;

namespace CC_Backend.Repositories.Friends
{
    public class FriendsRepo : IFriendsRepo
    {
        private readonly NatureAIContext _context;

        public FriendsRepo(NatureAIContext context)
        {
            _context = context;
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

            foreach (var friend in friendsResult)
            {
                var viewModel = new FriendViewModel
                {
                    DisplayName = friend.DisplayName
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
        public async Task<(bool success, string message)> AddFriendAsync(string userId, string friendDisplayName)
        {
            try
            {
                var friendToAddId = await _context.Users
                    .Where(u => u.DisplayName == friendDisplayName)
                    .Select(u => u.Id)
                    .SingleOrDefaultAsync();

                // Check if the user is trying to add themselves as a friend.
                if (userId == friendToAddId)
                {
                    return (false, "You cannot add yourself as a friend.");
                }

                // Check if the users are already friends.
                if (_context.Friends.Any(f => f.FriendId1 == userId && f.FriendId2 == friendToAddId) ||
                    _context.Friends.Any(f => f.FriendId1 == friendToAddId && f.FriendId2 == userId))
                {
                    return (false, "Users are already friends.");
                }


                else
                {
                    var newFriend = new Models.Friends
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
        public async Task<(bool success, string message)> RemoveFriendAsync(string userId, string friendDisplayName)
        {
            try
            {
                var friendToDeleteId = await _context.Users
                    .Where(u => u.DisplayName == friendDisplayName)
                    .Select(u => u.Id)
                    .SingleOrDefaultAsync();

                var friendship = await _context.Friends
                    .Where(f => f.FriendId1 == userId && f.FriendId2 == friendToDeleteId || f.FriendId2 == userId && f.FriendId1 == friendToDeleteId)
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
    }
}
