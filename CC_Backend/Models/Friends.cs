using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class Friends
    {
        [ForeignKey("UserId")]
        public int FriendId1 { get; set; }
        [ForeignKey("UserId")]
        public int FriendId2 { get; set; }
    }
}
