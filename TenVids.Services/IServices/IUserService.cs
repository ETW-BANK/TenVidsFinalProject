
using TenVids.Models;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
   public  interface IUserService
    {
        Task <IEnumerable<UserDisplayVM>> GetAllUsersAsync ();

        Task<UserAddEditVM> AddUserAsync(string id);

        Task<ApplicationUser> CreateUser(UserAddEditVM model);
        Task<(bool IsValid, UserAddEditVM Model)> ValidateUserForAddAsync(UserAddEditVM model);
        Task<List<string>> GetApplicationRols();
        Task<bool> NameExists(string name);
        Task<bool> EmailExists(string email);
    }
}
