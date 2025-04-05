
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

        public async Task CreateCategoryAsync(CategoryVM categoryVM)
        {
            if (string.IsNullOrWhiteSpace(categoryVM.Name))
            {
                throw new ArgumentException("Category name cannot be empty");
            }

            var category = new Category
            {
                Name = categoryVM.Name.Trim(),
              
            };

            _unitOfWork.CategoryRepository.Add(category);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
          category = _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == category.Id).Result;

            if (category==null)
            {
                throw new Exception("Category not found.");
            }
         
            _unitOfWork.CategoryRepository.Remove(category);
            await _unitOfWork.CompleteAsync();

        }

        public async Task<IEnumerable<CategoryVM>> GetAllCategoriesAsync()
        {
            var result=await _unitOfWork.CategoryRepository.GetAllAsync();
            result = result.OrderBy(c => c.Name);
            if (result == null)
            {
                return null;
            }
            return result.Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name,
               
            }).ToList();
        }

        public Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                throw new Exception("Category not found.");
            }


            return category;
        }

        public async void UpdateCategoryAsync(Category category)
        {
            category =await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);

            if (category == null)
            {
                throw new Exception("Category not found.");
            }
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Category name cannot be empty");
            }
            var categoryToUpdate = new Category
            {
                Id = category.Id,
                Name = category.Name

            };  


            _unitOfWork.CategoryRepository.Update(category,categoryToUpdate);
            await  _unitOfWork.CompleteAsync();
        }
    }
}
   

