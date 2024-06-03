using System.Collections.Generic;
using System.Linq;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;

namespace CC_Backend.Services
{
    public class StampService : IStampService
    {
        public List<StampViewModel> CreateStampViewModels(ICollection<StampCollected> stamps)
        {
            var stampsViewModels = new List<StampViewModel>();

            foreach (var stamp in stamps)
            {
                var stampViewModel = new StampViewModel
                {
                    Name = stamp.Stamp.Name,
                    Icon = stamp.Stamp.Icon
                };
                stampsViewModels.Add(stampViewModel);
            }
            return stampsViewModels;
        }

        public StampViewModel CreateSelectedStampViewModel(int stampId)
        {
            var stampViewModel = new StampViewModel
            {
                StampId = stampId,
                Name = stamp.Name,
                Facts = stamp.Facts,
                Rarity = stamp.Rarity,
                Icon = stamp.Icon,
                Latitude = stamp.Latitude,
                Longitude = stamp.Longitude,
                Category = new CategoryDTO
                {
                    Title = stamp.Category?.Title ?? ""
                }
            };
            return stampViewModel;
        }
    }
}