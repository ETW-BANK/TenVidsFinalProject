using TenVids.Models;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
   public  interface IUserService
    {
        Task <IEnumerable<UserDisplayVM>> GetAllUsersAsync ();
        Task<UserAddEditVM> AddUserAsync(string id);
        Task<ApplicationUser> CreateUser(UserAddEditVM model);
        Task<List<string>> GetApplicationRols();
        Task<bool> NameExists(string name);
        Task<bool> EmailExists(string email);

        Task<bool> LockUserAsync(string id);
        Task<bool> UnLockUserAsync(string id);
        Task<bool> DeleteUserAsync(string id);
    }
}
