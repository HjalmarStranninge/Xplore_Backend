using CC_Backend.Data;
using CC_Backend.Models;

namespace CC_Backend.Handlers
{
    public interface IStampHandler
    {
         Task<StampCollected> CreateStampCollected(string promptResult, string prompt, string userId, string[] coordinates);
    }
}
