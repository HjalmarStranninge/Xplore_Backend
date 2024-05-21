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
using CC_Backend.Repositories.User;

namespace CC_Backend.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly IUserRepo _IUserRepo;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IAccountService accountService, IUserRepo urepo)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _IUserRepo = urepo;
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
        public async Task<IActionResult> SetProfilePicture([FromBody]SetProfilePictureDTO dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                bool result = await _IUserRepo.SetUserProfile(userId, dto.ProfilePicture);

                return Ok(result);




            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}
