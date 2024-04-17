namespace CC_Backend.Models
{
    public class FriendList
    {
        private int _friendListId { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
