using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using FluentValidation.Results;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using CC_Backend.Models.Viewmodels;
using Microsoft.AspNetCore.Authentication.Cookies;
using CC_Backend.Repositories.UserRepo;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using CC_Backend.Services;
using FluentValidation;

namespace CC_Backend.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IValidator<RegisterDTO> _validator;
        private readonly IValidator<SendPasswordResetTokenDto> _sendPasswordResetTokenValidator;
        private readonly IValidator<ResetPasswordDTO> _resetPasswordValidator;
        private readonly IAccountService _accountService;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IUserRepo _userRepo;
        private readonly IEmailService _emailService;
        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, IAccountService accountService, IJwtAuthManager jwtAuthManager, IUserRepo userRepo, IEmailService emailService, IUserService userService, IValidator<RegisterDTO> validator, IValidator<ResetPasswordDTO> resetPasswordValidator, IValidator<SendPasswordResetTokenDto> sendPasswordTokenValidator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _jwtAuthManager = jwtAuthManager;
            _userRepo = userRepo;
            _emailService = emailService;
            _userService = userService;
            _validator = validator;
            _resetPasswordValidator = resetPasswordValidator;
            _sendPasswordResetTokenValidator = sendPasswordTokenValidator;
        }

        // Add a new user
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(dto);
            if(!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var createUserResult = await _userService.RegisterUserAsync(dto);
            if(!createUserResult.Succeeded)
            {
                return BadRequest(createUserResult.Errors);
            }
            return Ok("Registration successful.");
 
        }

        // Log in the user
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await _accountService.Login(dto);

            if (result == null)
            {
                return BadRequest("Invalid login credentials");
            }

            return Ok(result);

        }

        // Log out a user
        [HttpPost("logout")]
        [Authorize]
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

        // Log in with Google
        [HttpGet("login-google")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            // Redirects the user to Google for authentication.
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Response from Google
        [HttpGet("googleresponse")]
        [AllowAnonymous]
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

        // Reset a users password
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            ValidationResult validationResult = await _resetPasswordValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _userService.ResetPasswordAsync(dto);

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("Password reset successfull.");
        }

        // Send a reset password token 
        [HttpPost("sendpasswordresettoken")]
        public async Task<IActionResult> SendPasswordResetToken([FromBody] SendPasswordResetTokenDto dto)
        {
            ValidationResult validationResult = await _sendPasswordResetTokenValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var (success, message) = await _userService.SendPasswordResetTokenAsync(dto);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(message);
        }
    }
}
