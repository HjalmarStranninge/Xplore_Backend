using System.ComponentModel.DataAnnotations;

namespace CC_Backend.Models
{
    public class Geodata
    {
        [Key]
        public int GeodataId { get; set; }
        public string[] Coordinates { get; set; }
        public DateTime DateWhenCollected { get; set; }
    }
}
