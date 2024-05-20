using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;
using System.Security.Claims;

namespace CC_Backend.Services
{
    public interface IJwtAuthManager
    {
        Task<JwtAuthResultViewModel> GenerateTokens(ApplicationUser user, IEnumerable<Claim> claims, DateTime now);
    }
}
