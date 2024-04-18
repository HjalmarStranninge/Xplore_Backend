using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class Icon
    {
        [Key]
        public int IconId { get; set; }
        [ForeignKey("Stamp")]
        public int StampId { get; set; }
        public virtual Stamp Stamp { get; set; }
        public byte[] IconData { get; set; }
    }
}
