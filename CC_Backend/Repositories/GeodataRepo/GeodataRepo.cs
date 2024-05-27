using CC_Backend.Data;
using CC_Backend.Models;

using Microsoft.AspNetCore.Http.HttpResults;


namespace CC_Backend.Repositories.GeodataRepo
{
    public class GeodataRepo : IGeodataRepo
    {
        private readonly NatureAIContext _context;

        public GeodataRepo(NatureAIContext context)
        {
            _context = context;
        }

        // Generates a new geodata object and saves it to the database.
        public async Task<Geodata> GetGeodataAsync()
        {
            // Coordinates are hard coded for now, to be replaced with actual coordinates where a stamp was collected later.
            var geodata = new Geodata()
            {
                Coordinates = ["59.2930° N", "18.0837° E"],
                DateWhenCollected = DateTime.Now
            };

            try
            {
                _context.GeoData.Add(geodata);
                await _context.SaveChangesAsync();
                return geodata;
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't save new geodata object in database.", ex);
            }
        }
    }
}
