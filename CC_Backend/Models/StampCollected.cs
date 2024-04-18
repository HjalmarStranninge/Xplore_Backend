using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class StampCollected
    {
        [Key]
        public int StampCollectedId { get; set; }
        [ForeignKey("Stamp")]
        public int StampId { get; }
        public virtual Geodata Geodata { get; set; }
        public virtual Stamp Stamp { get; set; }
        public virtual User User { get; set; }

       
    }
}
