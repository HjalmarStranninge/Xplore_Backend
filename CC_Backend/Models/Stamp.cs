using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class Stamp
    {
        [Key]
        public int StampId { get; set; }
        public string Name { get; set; }
        public string[] Facts { get; set; }
        public double Rarity { get; set; }
        public virtual Icon Icon { get; set; }
        public virtual Category Category { get; set; }

    }
}
