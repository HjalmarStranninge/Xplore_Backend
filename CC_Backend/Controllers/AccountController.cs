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

namespace CC_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IAccountService accountService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/register")]
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
        [Route("/login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await _accountService.Login(dto);

            if (result == null)
            {
                return BadRequest("Invalid login credentials");
            }

            return Ok(result);

        }

        // Logs out the currently authenticated user.
        [HttpPost("/logout")] // This route will match "/logout"
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
    }
}
