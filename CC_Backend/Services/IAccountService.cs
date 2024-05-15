using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Services
{
    public interface IAccountService
    {
        Task<LoginResultViewModel> Login(LoginDTO dto);
    }
}
