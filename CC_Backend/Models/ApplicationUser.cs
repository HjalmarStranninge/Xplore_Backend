using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace CC_Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<StampCollected>? StampsCollected { get; set; }
    }
}
