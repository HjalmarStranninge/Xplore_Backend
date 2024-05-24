using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class Friends
    {

        [Key] 
        public int Id { get; set; }
        [ForeignKey("User")]
        public string FriendId1 { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("User2")]
        public string FriendId2 { get; set; }

        public ApplicationUser User2 { get; set; }
    }
}
