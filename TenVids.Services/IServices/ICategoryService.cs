

using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
   public interface ICategoryService
    {

        Task<IEnumerable<CategoryVM>> GetAllCategories();
    }
}
