using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CC_Backend.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
    
        public string Title { get; set; }
        public virtual ICollection<Stamp> Stamps { get; set; }
    }
}
