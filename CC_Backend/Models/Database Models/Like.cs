using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CC_Backend.Models
{
    public class Like
    {

        [Key]
        public int LikeId { get; set; }

        [ForeignKey("StampCollected")]
        public int StampCollectedId { get; set; }
        public StampCollected StampCollected { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
