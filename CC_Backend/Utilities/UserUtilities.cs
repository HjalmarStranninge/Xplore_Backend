using System.Security.Claims;
using CC_Backend.Models;

namespace CC_Backend.Utilities
{
    public static class UserUtilities
    {
        public static string ExtractUserIdFromToken(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return userId;
        }
    }
}
