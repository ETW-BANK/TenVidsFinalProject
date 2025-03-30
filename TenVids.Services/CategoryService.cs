
using TenVids.Models;
using TenVids.Repository;
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

        public async Task<CategoryVM> CreateCategoryAsync(CategoryVM categoryVM)
        {
            var category = new Category
            {
                Name = categoryVM.Name
            };

            var createdCategory = await _unitOfWork.CategoryRepository.CreateCategoryAsync(category);
            return new CategoryVM
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name
            };
        }


        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _unitOfWork.CategoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<IEnumerable<CategoryVM>> GetAllCategoriesAsync()
        {
            var categries = await _unitOfWork.CategoryRepository.GetAllCategories();

            if (categries == null)
            {
                return null;
            }
            else
            {
                var categorylist = categries.Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name

                }).ToList();

                return categorylist;
            }

        }

        public async Task<CategoryVM> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            return category == null ? null : new CategoryVM
            {
                Id = category.Id,
                Name = category.Name
            };
        }


        public async Task<CategoryVM> UpdateCategoryAsync(CategoryVM categoryVM)
        {
            var category = new Category
            {
                Id = categoryVM.Id,
                Name = categoryVM.Name
            };

            var updatedCategory = await _unitOfWork.CategoryRepository.UpdateCategoryAsync(category);
            return updatedCategory == null ? null : new CategoryVM
            {
                Id = updatedCategory.Id,
                Name = updatedCategory.Name
            };
        }
    }
}
   

