using CC_Backend.Models.Viewmodels;
using CC_Backend.Models;

namespace CC_Backend.Services
{
    public interface IStampService
    {
        List<StampViewModel> CreateStampViewModels(ICollection<StampCollected> stamps);
    }
}
