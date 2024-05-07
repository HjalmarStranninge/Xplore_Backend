namespace CC_Backend.Models.DTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string newPassword { get; set; }
    }
}
