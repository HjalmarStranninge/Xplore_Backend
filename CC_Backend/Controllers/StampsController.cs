using CC_Backend.Models;
using CC_Backend.Repositories.Stamps;
using CC_Backend.Repositories.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Org.BouncyCastle.Asn1.Cms;
using System.Security.Claims;

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
                // Extract logged in user from token.
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var result = await _iStampRepo.GetStampsFromUserAsync(userId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/selectstamp")]
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

                // Retrive information about the selected stamp
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

    }
}
