
using TenVids.Models;
using TenVids.Repository.IRepository;
using TenVids.Services.IServices;
using TenVids.ViewModels;

namespace TenVids.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task CreateCategoryAsync(CategoryVM categoryVM)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCategoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoryVM>> GetAllCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CategoryVM> GetCategoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategoryAsync(CategoryVM categoryVM)
        {
            throw new NotImplementedException();
        }
    }
}
   

