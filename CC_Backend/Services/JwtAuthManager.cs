using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace CC_Backend.Services
{
    public class JwtAuthManager : IJwtAuthManager
    {
        public UserManager<ApplicationUser> userManager { get; }
        private readonly byte[] _secret;
        private readonly IConfiguration _config;

        public JwtAuthManager(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            _config = configuration;
            _secret = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"));
            
        }
        public async Task<JwtAuthResultViewModel> GenerateTokens(ApplicationUser user, DateTime now)
        {
            var jwtToken = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                expires: now.AddMinutes(180),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));

            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            
            return new JwtAuthResultViewModel
            {
                AccessToken = accessTokenString,
            };

                
        }
    }
}
