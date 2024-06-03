using System.Collections.ObjectModel;

namespace CC_Backend.Models.Viewmodels
{
    public class UserProfileViewmodel
    {
        public string DisplayName { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public int StampsCollectedTotalCount { get; set; }
        public int FriendsCount { get; set; }

        public ICollection<StampViewModel>? StampCollectedTotal { get; set; }
        public IReadOnlyList<FriendViewModel>? Friends { get; set; }

    }
}
