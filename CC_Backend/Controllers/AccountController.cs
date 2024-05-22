using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using FluentValidation.Results;
using CC_Backend.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using CC_Backend.Models.Viewmodels;
using Microsoft.AspNetCore.Authentication.Cookies;
using CC_Backend.Repositories.User;

namespace CC_Backend.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IUserRepo _userRepo;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IAccountService accountService, IJwtAuthManager jwtAuthManager, IUserRepo userRepo)

        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _jwtAuthManager = jwtAuthManager;
            _userRepo = userRepo;
        }

        // Add a new user
        [HttpPost]
        [AllowAnonymous]
        [Route("account/register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var validator = new RegisterDTOValidator();
            ValidationResult result = validator.Validate(dto);

            if (result.IsValid)
            {
                var user = new ApplicationUser { DisplayName = dto.DisplayName, Email = dto.Email, UserName = dto.Email };
                var createUserResult = await _userManager.CreateAsync(user, dto.Password);
                if (createUserResult.Succeeded)
                {
                    return Ok("Registration successful");
                }
                else
                {
                    return BadRequest(createUserResult.Errors);
                }
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        // Log in the user
        [HttpPost]
        [AllowAnonymous]
        [Route("account/login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await _accountService.Login(dto);

            if (result == null)
            {
                return BadRequest("Invalid login credentials");
            }

            return Ok(result);

        }

        // Log in with Google
        [HttpGet]
        [AllowAnonymous]
        [Route("account/login-google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Response from Google
        [HttpGet]
        [AllowAnonymous]
        [Route("account/googleresponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims.ToList();
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var displayName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (claims == null)
            {
                return RedirectToAction(nameof(Login));
            }           

            if (email == null || displayName == null)
            {
                return BadRequest("Error retrieving email or display name from the external provider.");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var userClaims = await _accountService.GetUserClaims(user);
                var jwtResult = await _jwtAuthManager.GenerateTokens(user, userClaims, DateTime.UtcNow);

                await _userManager.SetAuthenticationTokenAsync(
                    user,
                    "Authentication",
                    "Bearer",
                    jwtResult.RefreshToken.TokenString);

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
                return Ok(loginResult);
            }

            else
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

                    if (identityResult.Succeeded)
                    {
                        user = newUser;
                        var userClaims = await _accountService.GetUserClaims(user);
                        var jwtResult = await _jwtAuthManager.GenerateTokens(user, userClaims, DateTime.UtcNow);

                        await _userManager.SetAuthenticationTokenAsync(
                            user,
                            "Authentication",
                            "Bearer",
                            jwtResult.RefreshToken.TokenString);

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
                        return Ok(loginResult);
                    }
                }

                return BadRequest("Failed to create a new user account.");
            }
        }

        // Log out a user
        [HttpPost]
        [Authorize]
        [Route("account/logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok("User logged out.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred during logout: {ex.Message}");
            }
        }

        // Set a profile picture the a user
        [HttpPost]
        [Authorize]
        [Route("account/setprofilepicture")]
        public async Task<IActionResult> SetProfilePicture([FromBody]SetProfilePictureDTO dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                bool result = await _userRepo.SetUserProfile(userId, dto.ProfilePicture);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}
