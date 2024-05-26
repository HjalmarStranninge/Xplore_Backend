using CC_Backend.Data;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using Microsoft.EntityFrameworkCore;

namespace CC_Backend.Repositories.FriendsRepo
{
    public class FriendsRepo : IFriendsRepo
    {
        private readonly NatureAIContext _context;

        public FriendsRepo(NatureAIContext context)
        {
            _context = context;
        }

        // Get a list of all of a users friends
        public async Task<IReadOnlyList<FriendViewModel>> GetFriendsAsync(string userId)
        {
            var friends1 = await _context.Friends
                .Where(f => f.FriendId1 == userId)
                .Select(f => f.FriendId2)
                .ToListAsync();

            var friends2 = await _context.Friends
                .Where(f => f.FriendId2 == userId)
                .Select(f => f.FriendId1)
                .ToListAsync();

            var friendIds = friends1.Concat(friends2).Distinct();

            var friendsResult = await _context.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToListAsync();

            var friends = new List<FriendViewModel>();

            foreach (var friend in friendsResult)
            {
                var viewModel = new FriendViewModel
                {
                    DisplayName = friend.DisplayName,
                    ProfilePicture = friend.ProfilePicture

                };
                friends.Add(viewModel);
            }
            return friends;

        }

        // Add a new friend
        public async Task<(bool success, string message)> AddFriendAsync(string userId, AddFriendDTO dto)
        {
            try
            {
                var friendToAddId = await _context.Users
                    .Where(u => u.Email == dto.FriendEmail)
                    .Select(u => u.Id)
                    .SingleOrDefaultAsync();

                if (userId == friendToAddId)
                {
                    return (false, "You cannot add yourself as a friend.");
                }

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
        public async Task<(bool success, string message)> RemoveFriendAsync(string userId, RemoveFriendDTO dto)
        {
            try
            {
                var friendToDeleteId = await _context.Users
                    .Where(u => u.Email == dto.FriendEmail)
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
