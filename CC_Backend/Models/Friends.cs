using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class Friends
    {
        [ForeignKey("User")]
        public int FriendId1 { get; set; }
        public User User { get; set; }

        [ForeignKey("User2")]
        public int FriendId2 { get; set; }

        public User User2 { get; set; }
    }
}
