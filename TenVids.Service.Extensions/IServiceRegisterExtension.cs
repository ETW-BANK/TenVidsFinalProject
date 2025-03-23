using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace TenVids.Service.Extensions
{
    public interface IServiceRegisterExtension
    {

        void RegisterServices(IServiceCollection services, IConfiguration configuration);
    }
}
