using System.ComponentModel.DataAnnotations;

namespace CC_Backend.Models
{
    public class FriendList
    {
        [Key]
        public int FriendListId { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
