using CC_Backend.Data;
using CC_Backend.Models;

namespace CC_Backend.Handlers
{
    public class StampHandler : IStampHandler
    {
        private readonly NatureAIContext _context;

        public StampHandler(NatureAIContext context)
        {
            _context = context;
        }

        // Convert a stamps to a collected stamp
        public StampCollected CreateStampCollected(string promptResult, string prompt, string userId)
        {
            int resultValue;
            try
            {
                resultValue = int.Parse(promptResult);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex}");
            }

            if (resultValue >= 75)
            {
                try
                {
                    var stamp = _context.Stamps
                    .Where(s => s.Name.ToLower() == prompt.ToLower())
                    .SingleOrDefault();

                    var stampCollected = new StampCollected
                    {
                        Stamp = stamp,
                    };

                    return stampCollected;
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occured when fetching stamp: {ex}");
                }

            }

            else
            {
                throw new Exception("Picture doesn't match prompt.");
            }
        }
    }
}
