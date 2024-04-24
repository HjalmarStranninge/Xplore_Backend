using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CC_Backend.Models
{
    public class User
    {
        public int UserId { get; set; }

        [ForeignKey("CredentialsId")]
        public int CredentialsId { get; set; }
        public virtual ICollection<StampCollected>? StampsCollected { get; set; }
    }
}
