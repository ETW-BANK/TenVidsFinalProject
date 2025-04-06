
using TenVids.Models;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
   public interface ICategoryService
    {

        Task<IEnumerable<CategoryVM>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<ErrorModel<Category>> CreateCategoryAsync(CategoryVM model);
        Task<ErrorModel<Category>> UpdateCategoryAsync(CategoryVM model);
        Task DeleteCategoryAsync(Category category);
      


    }
}
