using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class Friends
    {
        [ForeignKey("User")]
        public string FriendId1 { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("User2")]
        public string FriendId2 { get; set; }

        public ApplicationUser User2 { get; set; }
    }
}
