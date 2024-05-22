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
using Microsoft.EntityFrameworkCore.Storage.Internal;

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

        [HttpGet]
        [AllowAnonymous]
        [Route("account/login-google")]
        public IActionResult GoogleLogin()
        {
            // Redirects the user to Google for authentication.
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("account/googleresponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            // After authentication, the response is handled. Some user info is extracted from the claims.
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims.ToList();
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var displayName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Error handling.
            if (claims == null)
            {
                return RedirectToAction(nameof(Login));
            }

            if (email == null || displayName == null)
            {
                return BadRequest("Error retrieving email or display name from the external provider.");
            }

            var user = await _userManager.FindByEmailAsync(email);

            // Logs in the user and generates a JWT token if a user matching the Google mail is found.
            if (user != null)
            {
                return Ok(await _accountService.SignInExistingUser(user));
            }

            // If a user couldn't be found, a new one is created and then signed in.
            else
            {
                var loginResult = await _accountService.RegisterAndSignInNewUser(email, displayName);
                if (loginResult != null)
                {
                    return Ok(loginResult);
                }
                return BadRequest("Failed to create a new user account.");
            }
        }

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

        [HttpPost]
        [Authorize]
        [Route("account/setprofilepicture")]
        public async Task<IActionResult> SetProfilePicture([FromBody] SetProfilePictureDTO dto)
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
