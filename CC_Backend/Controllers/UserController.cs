using CC_Backend.Models;
using CC_Backend.Repositories.User;
using CC_Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace CC_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepo _iUserRepo;
        private readonly IEmailService _emailService;

        public UserController(IUserRepo repo,IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _iUserRepo = repo;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        [Route("/getallusers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _iUserRepo.GetAllUsersAsync();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("/resetpassword")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return StatusCode(500, "Email not found!");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _emailService.SendEmailAsync(token);





                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}
