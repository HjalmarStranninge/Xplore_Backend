using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Services
{
    public interface IJwtAuthManager
    {
        Task<JwtAuthResultViewModel> GenerateTokens(ApplicationUser user, DateTime now);
    }
}
