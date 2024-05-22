using CC_Backend.Models.Viewmodels;
using CC_Backend.Models;

namespace CC_Backend.Services
{
    public interface ISearchUserService
    {
        List<SearchUserViewModel> GetSearchUserViewModels(List<ApplicationUser> users, string query);
    }
}
