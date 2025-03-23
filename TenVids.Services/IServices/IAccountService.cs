using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface IAccountService
    {
        Task<LoginVM> Login(string returnUrl);

    }
}
