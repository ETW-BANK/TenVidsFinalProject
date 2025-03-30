

using TenVids.Models;

namespace TenVids.Repository.IRepository
{
   public interface ICategoryRepository:IRepository<Category>
    {
        Task <IEnumerable<Category>> GetAllCategories();
        Task<Category> UpdateCategoryAsync(Category category);
        Task<Category> CreateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
