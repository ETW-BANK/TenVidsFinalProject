
using Microsoft.EntityFrameworkCore;
using TenVids.Models;
using TenVids.Repository.IRepository;
using TenVids.Services.IServices;
using TenVids.Utilities;
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
            result = result.OrderBy(c => c.Id);
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

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                throw new Exception("Category not found.");
            }

            return category;
        }


        public async Task<ErrorModel<Category>> UpdateCategoryAsync(CategoryVM model)
        {
       
            var existingCategory = await _unitOfWork.CategoryRepository
                .GetFirstOrDefaultAsync(x => x.Id == model.Id, tracked: true);

            if (existingCategory == null)
            {
                return ErrorModel<Category>.Failure("Category doesn't exist.", 404);
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return ErrorModel<Category>.Failure("Category name cannot be empty.", 400);
            }

            var duplicateExists = await _unitOfWork.CategoryRepository
                .GetFirstOrDefaultAsync(x =>
                    x.Name.ToLower() == model.Name.ToLower() &&
                    x.Id != model.Id);

            if (duplicateExists != null)
            {
                return ErrorModel<Category>.Failure("Another category with this name already exists.", 409);
            }

           
            existingCategory.Name = model.Name.Trim();
           

            try
            {
                await _unitOfWork.CompleteAsync();
                return ErrorModel<Category>.Success(existingCategory, "Category updated successfully");
            }
            catch (DbUpdateException ex)
            {
                
                return ErrorModel<Category>.Failure("Failed to update category due to database error.", 500);
            }
        }

        public async Task<ErrorModel<Category>> CreateCategoryAsync(CategoryVM model)
        {
            var CategoryExists = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(x => x.Name == model.Name);

            if (CategoryExists != null)
            {
                return ErrorModel<Category>.Failure("Category with this name already exists.", 409);
            }
            var newcCategory = new Category
            {
                Name = model.Name,
              
            };
            _unitOfWork.CategoryRepository.Add(newcCategory);
            await _unitOfWork.CompleteAsync();

            return ErrorModel<Category>.Success(newcCategory, "Category Created Succesfully");
        }
    }
}
   

