using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CC_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/registeraccount")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { DisplayName = dto.DisplayName, Email = dto.Email, UserName = dto.Email };
                var result = await _userManager.CreateAsync(user, dto.Password);
                if (result.Succeeded)
                {
                    return Ok("Registration successful");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            return BadRequest(ModelState);
        }

        /// Logs out the currently authenticated user.
        [HttpPost("~/logout")] // This route will match "/logout"
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
