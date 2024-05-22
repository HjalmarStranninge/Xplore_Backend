using CC_Backend.Models.Viewmodels;
using CC_Backend.Models;

namespace CC_Backend.Services
{
    public class SearchUserService : ISearchUserService
    {
        public List<SearchUserViewModel> GetSearchUserViewModels(List<ApplicationUser> users, string query)
        {
            return users
                .Select(u => new SearchUserViewModel
                {
                    DisplayName = u.DisplayName,
                    ProfilePicture = u.ProfilePicture
                })
                .Where(u => u.DisplayName.Contains(query))
                .Take(5)
                .ToList();
        }
    }
}
