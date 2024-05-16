using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace CC_Backend.Services
{
    public class JwtAuthManager : IJwtAuthManager
    {
        public UserManager<ApplicationUser> _userManager { get; }
        private readonly byte[] _secret;
        private readonly IConfiguration _config;

        public JwtAuthManager(UserManager<ApplicationUser> userManager, IConfiguration configuration, string secretKey)
        {
            this._userManager = userManager;
            _config = configuration;
            _secret = Encoding.ASCII.GetBytes(secretKey);

        }

        // Generates new access and refresh tokens.
        public async Task<JwtAuthResultViewModel> GenerateTokens(ApplicationUser user, DateTime now)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var jwtToken = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                claims: claims,
                expires: now.AddMinutes(Convert.ToInt32(Environment.GetEnvironmentVariable("ACCESSTOKEN_EXPIRY"))),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));

            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var refreshTokenString = await _userManager.GenerateUserTokenAsync(user, "Default", "RefreshToken");

            var refreshTokenModel = new RefreshTokenViewModel
            {
                Email = user.Email,
                TokenString = refreshTokenString,
                ExpireAt = now.AddMinutes(Convert.ToInt32(Environment.GetEnvironmentVariable("REFRESHTOKEN_EXPIRY")))
            };

            return new JwtAuthResultViewModel
            {
                AccessToken = accessTokenString,
                RefreshToken = refreshTokenModel
            };


        }
    }
}
