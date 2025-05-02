
using TenVids.Models;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
   public  interface IUserService
    {
        Task <IEnumerable<UserDisplayVM>> GetAllUsersAsync ();

        Task<UserAddEditVM> AddUserAsync(string id);

        Task<ApplicationUser> CreateUser(UserAddEditVM model);
    }
}
