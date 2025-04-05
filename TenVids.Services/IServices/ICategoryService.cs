

using TenVids.Models;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
   public interface ICategoryService
    {

        Task<IEnumerable<CategoryVM>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(CategoryVM categoryVM);
        void UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);


    }
}
