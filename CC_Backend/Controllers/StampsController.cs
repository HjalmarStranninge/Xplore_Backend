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
        private readonly IStampsRepo _iStampRepo;

        public StampsController(IStampsRepo repo, UserManager<ApplicationUser> userManager)
        {
            _iStampRepo = repo;
            _userManager = userManager;
        }

        // Get all stamps from a user
        [HttpGet("stamps/getstampsfromuser")]
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
                var result = await _iStampRepo.GetStampsFromUserAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Get a selected stamp and the information
        [HttpGet("stamps/selectstamp")]
        [Authorize]
        public async Task<ActionResult<Stamp>> GetSelectStamp(int stampId)
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
                var stamp = await _iStampRepo.GetSelectedStamp(stampId);
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
        [HttpGet("stamps/categorystampscount")]
        [Authorize]
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
                var categoryStamps = await _iStampRepo.GetCategoryStampCountsAsync();
                return Ok(categoryStamps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Add a new stamp with a new category
        [HttpPost("stamps/addstampwithnewcategory")]
        [AllowAnonymous]
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
                var result = await _iStampRepo.CreateStampAsync(stampToAdd);
                await _iStampRepo.AddStampToCategoryAsync(stampToAdd, dto.Category.Title);
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
                Category categoryToAdd = await _iStampRepo.FindCategoryWithStampAsync(dto.CategoryTitle);

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
                var result = await _iStampRepo.CreateStampAsync(stampToAdd);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Get category with all stamps
        [HttpGet("stamps/getcategorywithstamps")]
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

                var (categoryDto, message) = await _iStampRepo.GetStampsFromCategoryAsync(categoryId);

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
