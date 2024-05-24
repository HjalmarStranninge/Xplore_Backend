using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Repositories.Stamps;
using CC_Backend.Repositories.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CC_Backend.Controllers
{
    [ApiController]
    public class StampsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStampsRepo _stampsRepo;

        public StampsController(IStampsRepo stampsRepo, UserManager<ApplicationUser> userManager)
        {
            _stampsRepo = stampsRepo;
            _userManager = userManager;
        }

        // Gets all of a users collected stamps
        [HttpGet]
        [Route("stamps/getuserscollectedstamps")]
        [Authorize]
        public async Task<IActionResult> GetStampsFromUser()
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                // Retrieve information about stamps from the user
                var result = await _stampsRepo.GetStampsFromUserAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Get the information of a selected stamp
        [HttpGet]
        [Authorize]
        [Route("stamps/getstampinfo")]
        public async Task<ActionResult<Stamp>> SelectStamp(int stampId)
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                // Retrieve information about the selected stamp
                var stamp = await _stampsRepo.GetSelectedStampAsync(stampId);
                if (stamp == null)
                    return NotFound("Stamp not found.");

                return Ok(stamp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Get how many stamps a user has collected from all stamps in a category
        [HttpGet]
        [Authorize]
        [Route("stamps/collectedincategorycount")]

        public async Task<IActionResult> GetCategoryStampsCount()
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                // Retrieve information about a category and how many stamps you have collected
                var categoryStamps = await _stampsRepo.GetCategoryStampCountsAsync();
                return Ok(categoryStamps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Add a new stamps to a new category
        [HttpPost]
        [AllowAnonymous]
        [Route("stamps/addstampwithnewcategory")]
        public async Task<IActionResult> CreateAStampAndCategory([FromBody] StampDTO dto)
        {
            try
            {
                var stampToAdd = new Stamp
                {
                    Name = dto.Name,
                    Facts = dto.Facts,
                    Rarity = dto.Rarity,
                    Icon = dto.Icon,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    Category = new Category
                    {
                        Title = dto.Category.Title,
                        Stamps = new List<Stamp>()
                    }
                };
                var result = await _stampsRepo.CreateStampAsync(stampToAdd);
                await _stampsRepo.AddStampToCategoryAsync(stampToAdd, dto.Category.Title);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Add a new stamp to an existing category
        [HttpPost("stamps/addstamptoexistingcategory")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAStampInCategory([FromBody] CreateStampInCategoryDTO dto)
        {
            try
            {
                Category categoryToAdd = await _stampsRepo.FindCategoryWithStampAsync(dto.CategoryTitle);

                var stampToAdd = new Stamp
                {
                    Name = dto.Name,
                    Facts = dto.Facts,
                    Rarity = dto.Rarity,
                    Icon = dto.Icon,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    Category = categoryToAdd
                };
                var result = await _stampsRepo.CreateStampAsync(stampToAdd);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Get all stamps in a category
        [HttpGet("stamps/getallstampsincategory")]
        [Authorize]
        public async Task<IActionResult> GetCategoryWithAllStamps(int categoryId)
        {
            try
            {
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var (categoryDto, message) = await _stampsRepo.GetStampsFromCategoryAsync(categoryId);

                if (categoryDto == null)
                {
                    return NotFound(new { Message = message });
                }
                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
