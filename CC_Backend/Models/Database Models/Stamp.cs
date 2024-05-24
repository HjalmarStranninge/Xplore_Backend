using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class Stamp
    {
        [Key]
        public int StampId { get; set; }
        public string Name { get; set; }
        public string? Facts { get; set; }
        public double? Rarity { get; set; }
        public byte[]? Icon { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
