namespace CC_Backend.Models.Viewmodels
{
    public class JwtAuthResultViewModel
    {
        public string AccessToken { get; set; }
        public RefreshTokenViewModel? RefreshToken { get; set; }
    }
}
