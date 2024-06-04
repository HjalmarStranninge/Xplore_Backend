using System.Collections.Generic;
using System.Linq;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Repositories.StampsRepo;
using CC_Backend.Utilities;

namespace CC_Backend.Services
{
    public class StampService : IStampService
    {
        private readonly IStampsRepo _iStampRepo;

        public StampService(IStampsRepo iStampRepo)
        {
            _iStampRepo = iStampRepo;
        }

        public async Task<ICollection<StampViewModel>> CreateStampViewModelsOfUser(string userId)
        {
            var userStamps = await _iStampRepo.GetStampsFromUserAsync(userId);
            var stampsViewModels = new List<StampViewModel>();

            foreach (var stamp in userStamps)
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

        public async Task<SelectedStampViewModel> CreateSelectedStampViewModel(int stampId)
        {
            var stamp = await _iStampRepo.GetSelectedStampAsync(stampId);
            if (stamp == null)
            {
                return null;
            }

            var selectedStampViewModel = new SelectedStampViewModel
            {
                StampId = stampId,
                Name = stamp.Name,
                Facts = stamp.Facts,
                Rarity = StampUtility.ConvertRarityToString(stamp.Rarity),
                Icon = stamp.Icon,
                Latitude = stamp.Latitude,
                Longitude = stamp.Longitude,
                Category = new CategoryDTO
                {
                    Title = stamp.Category?.Title ?? ""
                }
            };
            return selectedStampViewModel;
        }

        public async Task<CategoryWithStampListViewModel> CreateListOfCategoryStampViewModel(int categoryId)
        {
            var category = await _iStampRepo.GetStampsFromCategoryAsync(categoryId);
            var categoryViewModel = new CategoryWithStampListViewModel
            {
                Title = category.Title,
                Stamps = category.Stamps.Select(stamps => new SelectedStampViewModel
                {
                    StampId = stamps.StampId,
                    Name = stamps.Name,
                    Facts = stamps.Facts,
                    Rarity = StampUtility.ConvertRarityToString(stamps.Rarity),
                    Icon = stamps.Icon,
                    Latitude = stamps.Latitude,
                    Longitude = stamps.Longitude,

                }).ToList()
            };
            return (categoryViewModel);
        }
    }
}