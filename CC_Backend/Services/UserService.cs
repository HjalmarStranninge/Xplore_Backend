using System.Security.Claims;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace CC_Backend.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.Email,
            };
            return await _userManager.CreateAsync(user, dto.Password);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description="Email not found"});
            }
            return await _userManager.ResetPasswordAsync(user, dto.Token, dto.newPassword);
        }

        public async Task<(bool success, string message)> SendPasswordResetTokenAsync(SendPasswordResetTokenDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return (false, "Email not found.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _emailService.SendEmailAsync(token, user.Email, user.UserName);
        }
    }
}
