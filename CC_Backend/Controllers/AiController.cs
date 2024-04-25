using CC_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;

namespace CC_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController:ControllerBase
    {
        private readonly IOpenAIService _openAIService;

        public AiController(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        // Post endpoint for receiving a image with a prompt and returning a response.
        [HttpPost]
        [Route("/readimage")]
        public async Task<IActionResult> ReadImage([FromBody]ImageRequestDTO request)
        {
            try
            {
                var result = await _openAIService.ReadImage(request.Bytes, request.Prompt);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
