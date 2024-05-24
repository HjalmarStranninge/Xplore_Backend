using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC_Backend.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [ForeignKey("StampCollected")]
        public int StampCollectedId { get; set; }
        public StampCollected StampCollected { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
