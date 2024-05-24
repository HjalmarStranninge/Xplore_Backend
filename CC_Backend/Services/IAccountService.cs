using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CC_Backend.Services
{
    public interface IAccountService
    {
        Task<LoginResultViewModel> Login(LoginDTO dto);
        Task<IEnumerable<Claim>> GetUserClaims(ApplicationUser user);
        Task<LoginResultViewModel> SignInExistingUser(ApplicationUser user);
        Task<LoginResultViewModel> RegisterAndSignInNewUser(string email, string displayName);
    }
}
