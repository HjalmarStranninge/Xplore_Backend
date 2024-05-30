using CC_Backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace CC_Backend.Services
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDTO dto);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDTO dto);

        Task<(bool success, string message)> SendPasswordResetTokenAsync(SendPasswordResetTokenDto dto);

    }
}
