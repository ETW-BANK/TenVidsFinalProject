
using Microsoft.AspNetCore.Mvc;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface IAccountService
    {
        Task<LoginVM> Login(string returnUrl);

        Task<bool> LoginAsync(LoginVM loginVM);

        Task Register(RegisterVM registerVM);
        Task LogoutAsync();
    }
}
