using System.ComponentModel.DataAnnotations;

namespace CC_Backend.Models
{
    public class StampCollected
    {
        [Key]
        public int StampCollectedId { get; set; }
        public virtual Geodata? Geodata { get; set; }
        public virtual Stamp Stamp { get; set; }

        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

    }
}
