

using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
   public interface ICategoryService
    {

        Task<IEnumerable<CategoryVM>> GetAllCategoriesAsync();
        Task<CategoryVM> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(CategoryVM categoryVM);
        Task UpdateCategoryAsync(CategoryVM categoryVM);
        Task<bool> DeleteCategoryAsync(int id);
      
    }
}
