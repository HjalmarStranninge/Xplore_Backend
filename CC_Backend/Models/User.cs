using System.ComponentModel.DataAnnotations;
namespace CC_Backend.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public virtual Credentials Credentials { get; set; }
        public virtual ICollection<StampCollected>? StampsCollected { get; set; }
        public virtual FriendList? FriendList { get; set; }
    }
}
