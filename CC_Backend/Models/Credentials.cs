using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CC_Backend.Models
{
    public class Credentials
    {
        public int CredentialsId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual User User { get; set; }
    }
}
