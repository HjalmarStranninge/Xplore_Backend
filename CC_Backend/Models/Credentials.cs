using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CC_Backend.Models
{
    public class Credentials
    {
        [Key]
        public int CredentialsId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        private string _password { get; set; }
        private string _firebaseId { get; set; }
        public virtual User User { get; set; }
    }
}
