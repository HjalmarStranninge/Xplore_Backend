using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CC_Backend.Models
{
    public class Credentials
    {
        [Key]
        private int _credentialsId { get; set; }
        [ForeignKey("User")]
        private int _userId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        private string _password { get; set; }
        private string _firebaseId { get; set; }
        public virtual User User { get; set; }
    }
}
