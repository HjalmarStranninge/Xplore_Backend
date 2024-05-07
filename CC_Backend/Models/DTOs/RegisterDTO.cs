using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CC_Backend.Models.DTOs
{

    public class RegisterDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
