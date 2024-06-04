using Microsoft.EntityFrameworkCore;
using CC_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Data;
using CC_Backend.Models.DTOs;

namespace CC_Backend.Repositories.StampsRepo
{
    public interface IStampsRepo
    {
        Task<ICollection<StampCollected>> GetStampsFromUserAsync(string userId);
        Task<ICollection<StampCollected>> GetStampsCollectedFromUserAsync(string userId);
        Task AwardStampToUserAsync(string userId, StampCollected stamp);
        Task<Stamp> GetSelectedStampAsync(int stampId);
        Task<Category> GetStampsFromCategoryAsync(int categoryId);
        Task<CategoryViewModel> GetCategoryFromStampAsync(int categoryId);
        Task<List<CategoryWithStampsCountDTO>> GetCategoryStampCountsAsync();
        Task<bool> CreateStampAsync(Stamp stamp);
        Task AddStampToCategoryAsync(Stamp stamp, string categoryTitle);
        Task<Category> FindCategoryWithStampAsync(string categoryTitle);
        Task<StampCollected> GetStampCollectedAsync(int stampCollectedId);
    }
}
