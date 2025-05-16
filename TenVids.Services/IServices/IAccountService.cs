using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface IAccountService
    {
        Task<LoginVM> Login(string returnUrl);

        Task<ErrorModel<bool>> LoginAsync(LoginVM loginVM);

        Task Register(RegisterVM registerVM);
        Task LogoutAsync();
    }
}
