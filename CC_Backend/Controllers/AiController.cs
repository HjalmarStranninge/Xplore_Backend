using Microsoft.AspNetCore.Mvc;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Handlers;
using Microsoft.AspNetCore.Identity;
using CC_Backend.Repositories.Stamps;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CC_Backend.Services;

namespace CC_Backend.Controllers
{
    [ApiController]
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
        [HttpPost]
        [Authorize]
        [Route("ai/readimage")]
        public async Task<IActionResult> ReadImage([FromBody]ImageRequestDTO request)
        {
            try
            {
                var result = await _openAIService.ReadImage(request.Prompt,request.Picture);

                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var stampCollected = await _stampHandler.CreateStampCollected(result, request.Prompt, userId);
                await _stampsRepo.AwardStampToUserAsync(userId, stampCollected);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
