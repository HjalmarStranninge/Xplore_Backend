namespace CC_Backend.Models.Viewmodels
{
    public class RefreshTokenViewModel
    {
        public string Email { get; set; }
        public string TokenString { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
