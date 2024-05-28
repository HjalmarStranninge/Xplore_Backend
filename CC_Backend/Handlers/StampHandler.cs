using CC_Backend.Data;
using CC_Backend.Models;
using CC_Backend.Repositories.GeodataRepo;

namespace CC_Backend.Handlers
{
    public class StampHandler : IStampHandler
    {
        private readonly NatureAIContext _context;
        private readonly IGeodataRepo _geodataRepo;

        public StampHandler(NatureAIContext context, IGeodataRepo geodataRepo)
        {
            _context = context;
            _geodataRepo = geodataRepo;
        }

        // Mark a stamp as collected for a user
        public async Task<StampCollected> CreateStampCollected(string promptResult, string prompt, string userId)
        {
            string matchResult = null;
            try
            {
                var stamp = _context.Stamps
                .Where(s => s.Name.ToLower() == prompt.ToLower())
                .SingleOrDefault();

                var geodata = await _geodataRepo.GetGeodataAsync();

                var stampCollected = new StampCollected
                {
                    Stamp = stamp,
                    Geodata = geodata,
                };

                return stampCollected;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when fetching stamp: {ex}");
            }


            //int resultValue;
            //try
            //{
            //    resultValue = int.Parse(promptResult);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception($"An error occurred: {ex}");
            //}

            //if (resultValue >= 75)
            //{
            //    try
            //    {
            //        var stamp = _context.Stamps
            //        .Where(s => s.Name.ToLower() == prompt.ToLower())
            //        .SingleOrDefault();

            //        var geodata = await _geodataRepo.GetGeodataAsync();

            //        var stampCollected = new StampCollected
            //        {
            //            Stamp = stamp,
            //            Geodata = geodata,
            //        }; 

            //        return stampCollected;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception($"An error occurred when fetching stamp: {ex}");
            //    }

            //}

            //else
            //{
            //    throw new Exception("Picture doesn't match prompt.");
            //}
        }
    }
}
