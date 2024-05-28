using Microsoft.AspNetCore.Mvc;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CC_Backend.Services;
using CC_Backend.Repositories.StampsRepo;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CC_Backend.Controllers
{
    [ApiController]
    [Route("ai")]
    public class AIController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;
        private readonly IStampHandler _stampHandler;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStampsRepo _stampsRepo;

        public AIController(IOpenAIService openAIService, IStampHandler stampHandler, UserManager<ApplicationUser> userManager, IStampsRepo stampsRepo)
        {
            _openAIService = openAIService;
            _stampHandler = stampHandler;
            _userManager = userManager;
            _stampsRepo = stampsRepo;
        }

        // Post endpoint for receiving a image with a prompt, reading it and if it matches the prompt, awarding the user with the corresponding stamp.
        [HttpPost("readimage")]
        [Authorize]
        public async Task<IActionResult> ReadImage([FromBody] ImageRequestDTO request)
        {
            try
            {
                var result = await _openAIService.ReadImage(request.Prompt, request.Picture);

                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                //var stampCollected = await _stampHandler.CreateStampCollected(result, request.Prompt, userId);
                //await _stampsRepo.AwardStampToUserAsync(userId, stampCollected);

                string output = "false";
                int number = 0;

                if (int.TryParse(result, out number))
                {
                    // Switch on the parsed integer value
                    switch (number)
                    {
                        case int n when (n > 80 && n <= 100):
                            var stampCollected = await _stampHandler.CreateStampCollected(result, request.Prompt, userId);
                            await _stampsRepo.AwardStampToUserAsync(userId, stampCollected);
                            output = "true";
                            break;
                        case int n when (n >= 0 && n <= 60):
                            output = "false";
                            break;
                        case int n when (n > 60 && n <= 80):
                            output = "unsure";
                            break;
                        default:
                            // fixa något annat här
                            output = "false";
                            break;
                    }
                }
                else
                {
                    // Handle the case where parsing fails
                    output = "Invalid input. Please enter a valid number.";
                }

                return Ok(output);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
