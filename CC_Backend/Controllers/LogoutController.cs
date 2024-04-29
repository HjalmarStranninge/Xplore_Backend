using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using CC_Backend.Models;

namespace CC_Backend.Controllers
{
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        [HttpPost("~/logout")] // This route will match "/logout"
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                var response = new ApiResponse<object>
                {
                    Success = true,
                    Message = "Logout successful.",
                    Data = null
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during logout.",
                    Data = null
                };
                return BadRequest(response);
            }
        }
    }
}
