using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CC_Backend.Models.DTOs
{

    public class RegisterDTO
    {
        [Required(ErrorMessage ="Display name is required.")]
        public string DisplayName { get; set; }
        [Required(ErrorMessage = "Email adress is required.")]
        [EmailAddress(ErrorMessage = "Invalid email adress.")]
        public string Email { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
