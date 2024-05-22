using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

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

        // Login function
        public async Task<LoginResultViewModel> Login(LoginDTO dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);

            if (!result.Succeeded)
            {
                _logger.LogError($"PasswordSignInAsync failed");
                return null;
            }
            // Get user by email adress
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                _logger.LogError($"User with email {dto.Email} couldn't be found.");
                return null;
            }

            var userClaims = await GetUserClaims(user);
            var jwtResult = await _jwtAuthManager.GenerateTokens(user, userClaims, DateTime.Now);

            await _userManager.SetAuthenticationTokenAsync(
                 user,
                "Authentication",
                "Bearer",
                jwtResult.RefreshToken.TokenString);

            // Return information and the token
            return new LoginResultViewModel()
            {
                User = new UserViewModel()
                {
                    Email = dto.Email,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString,
                    DisplayName = user.DisplayName,
                    UserId = user.Id
                }
            };
        }

        public async Task<LoginResultViewModel> SignInExistingUser(ApplicationUser user)
        {
            var userClaims = await GetUserClaims(user);
            var jwtResult = await _jwtAuthManager.GenerateTokens(user, userClaims, DateTime.UtcNow);

            await _userManager.SetAuthenticationTokenAsync(user, "Authentication", "Bearer", jwtResult.RefreshToken.TokenString);

            var loginResult = new LoginResultViewModel
            {
                User = new UserViewModel
                {
                    Email = user.Email,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString,
                    DisplayName = user.DisplayName,
                    UserId = user.Id
                }
            };

            await _signInManager.SignInAsync(user, false);
            return loginResult;
        }

        public async Task<LoginResultViewModel> RegisterAndSignInNewUser(string email, string displayName)
        {
            var newUser = new ApplicationUser
            {
                DisplayName = displayName,
                Email = email,
                UserName = email
            };

            var identityResult = await _userManager.CreateAsync(newUser);

            if (identityResult.Succeeded)
            {
                var userClaims = await GetUserClaims(newUser);
                var jwtResult = await _jwtAuthManager.GenerateTokens(newUser, userClaims, DateTime.UtcNow);

                await _userManager.SetAuthenticationTokenAsync(newUser, "Authentication", "Bearer", jwtResult.RefreshToken.TokenString);

                var loginResult = new LoginResultViewModel
                {
                    User = new UserViewModel
                    {
                        Email = newUser.Email,
                        AccessToken = jwtResult.AccessToken,
                        RefreshToken = jwtResult.RefreshToken.TokenString,
                        DisplayName = newUser.DisplayName,
                        UserId = newUser.Id
                    }
                };

                await _signInManager.SignInAsync(newUser, false);
                return loginResult;
            }
            return null;
        }

        public async Task<IEnumerable<Claim>> GetUserClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            return claims;
        }

        // Refresh token 
        public async Task<JwtAuthResultViewModel> Refresh(ApplicationUser user, string refreshToken)
        {
            var isValid = await _userManager.VerifyUserTokenAsync(user, "Default", "RefreshToken", refreshToken);

            if (!isValid)
            {
                return null;
            }

            var claims = await GetUserClaims(user);
            return await _jwtAuthManager.GenerateTokens(user, claims, DateTime.UtcNow);
        }
    }
}
