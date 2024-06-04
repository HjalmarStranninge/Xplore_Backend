using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Repositories.StampsRepo;
using CC_Backend.Repositories.UserRepo;
using CC_Backend.Services;
using CC_Backend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CC_Backend.Controllers
{
    [ApiController]
    [Route("stamps")]
    public class StampsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStampsRepo _iStampRepo;
        private readonly IStampService _stampService;

        public StampsController(IStampsRepo repo, UserManager<ApplicationUser> userManager, IStampService stampService)
        {
            _iStampRepo = repo;
            _userManager = userManager;
            _stampService = stampService;
        }

        // Gets all of a users collected stamps
        [HttpGet("getuserscollectedstamps")]
        [Authorize]
        public async Task<IActionResult> GetStampsFromUser()
        {
            try
            {
                var userId = UserUtilities.ExtractUserIdFromToken(User);

                // Retrieve information about stamps from the user
                var result = await _stampService.CreateStampViewModelsOfUser(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Get the information of a selected stamp
        [HttpGet("getstampinfo")]
        [Authorize]
        public async Task<ActionResult<Stamp>> SelectStamp(int stampId)
        {
            try
            {
                var userId = UserUtilities.ExtractUserIdFromToken(User);

                // Retrieve information about the selected stamp
                var stamp = await _iStampRepo.GetSelectedStampAsync(stampId);
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
        [HttpGet("collectedincategorycount")]
        [Authorize]
        public async Task<IActionResult> GetCategoryStampsCount()
        {
            try
            {
                var userId = UserUtilities.ExtractUserIdFromToken(User);

                // Retrieve information about a category and how many stamps you have collected
                var categoryStamps = await _iStampRepo.GetCategoryStampCountsAsync();
                return Ok(categoryStamps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Add a new stamps to a new category
        [HttpPost("addstampwithnewcategory")]
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
        [HttpPost("addstamptoexistingcategory")]
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
        [HttpGet("getcategorywithstamps")]
        [Authorize]
        public async Task<IActionResult> GetCategoryWithAllStamps(int categoryId)
        {
            try
            {
                var userId = UserUtilities.ExtractUserIdFromToken(User);

                var categoryWithAllStamps= await _stampService.CreateListOfCategoryStampViewModel(categoryId);

                if (categoryId == null)
                {
                    return NotFound();
                }
                return Ok(categoryWithAllStamps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
