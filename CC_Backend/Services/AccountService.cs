using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;

namespace CC_Backend.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountService> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtAuthManager jwtAuthManager, ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
        }
        public async Task<LoginResultViewModel> Login(LoginDTO dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);

            if (!result.Succeeded)
            {
                _logger.LogError($"PasswordSignInAsync failed");
                return null;
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                _logger.LogError($"User with email {dto.Email} couldn't be found.");
                return null;
            }

            var jwtResult = await _jwtAuthManager.GenerateTokens(user, DateTime.Now);

            await _userManager.SetAuthenticationTokenAsync(
                 user,
                "Authentication",
                "Bearer",
                jwtResult.AccessToken);

            return new LoginResultViewModel()
            {
                User = new UserViewModel()
                {
                    Email = dto.Email,
                    AccessToken = jwtResult.AccessToken,
                    DisplayName = user.DisplayName,
                    UserId = user.Id
                }
            };
        }
    }
}
