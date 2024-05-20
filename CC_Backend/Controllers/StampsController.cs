using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Repositories.Stamps;
using CC_Backend.Repositories.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Org.BouncyCastle.Asn1.Cms;

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

        [HttpGet]
        [Route("stamps/getstampsfromuser")]
        [Authorize]

        public async Task<IActionResult> GetStampsFromUser()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                string userId = user.Id.ToString();
                var result = await _iStampRepo.GetStampsFromUserAsync(userId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("/selectstamp")]
        public async Task<ActionResult<Stamp>> SelectStamp(int stampId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

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

        [HttpGet]
        [Route("/getcategorywithstamps")]
        public async Task<IActionResult> GetCategoryWithStamps(int categoryId)
        {
            var categoryDto = await _iStampRepo.GetStampsFromCategory(categoryId);
            if (categoryDto.Item1 == null)
            {
                return NotFound();
            }
            return Ok(categoryDto.Item1);
        }
    }
}
