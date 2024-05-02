using CC_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Handlers;
using Microsoft.AspNetCore.Identity;
using CC_Backend.Repositories.Stamps;

namespace CC_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;
        private readonly IStampHandler _stampHandler;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStampsRepo _dBRepo;

        public AiController(IOpenAIService openAIService, IStampHandler stampHandler, UserManager<ApplicationUser> userManager, IStampsRepo dBRepo)
        {
            _openAIService = openAIService;
            _stampHandler = stampHandler;
            _userManager = userManager;
            _dBRepo = dBRepo;
        }

        // Post endpoint for receiving a image with a prompt, reading it and if it matches the prompt, awarding the user with the corresponding stamp.
        [HttpPost]
        [Route("/readimage")]
        public async Task<IActionResult> ReadImage([FromBody]ImageRequestDTO request)
        {
            try
            {
                var result = await _openAIService.ReadImage(request.Prompt);
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();

                var stampCollected = _stampHandler.CreateStampCollected(result, request.Prompt, userId);
                await _dBRepo.AwardStampToUserAsync(userId, stampCollected);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
