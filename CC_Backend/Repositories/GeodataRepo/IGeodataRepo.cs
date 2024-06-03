using CC_Backend.Models;

namespace CC_Backend.Repositories.GeodataRepo
{
    public interface IGeodataRepo
    {
        Task<Geodata> GetGeodataAsync(string[] coordinates);
    }
}
