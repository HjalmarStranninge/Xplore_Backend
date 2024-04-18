using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class Geodata
    {
        [Key]
        public int GeodataId { get; set; }
        [ForeignKey("StampCollected")]
        public int StampCollectedId { get; set; }
        public string[] Coordinates { get; set; }
        public DateTime DateWhenCollected { get; set; }
        public virtual StampCollected StampCollected { get; set; }
    }
}
