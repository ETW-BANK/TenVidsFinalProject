
using TenVids.Repository.IRepository;
using TenVids.Services.IServices;
using TenVids.ViewModels;

namespace TenVids.Services
{
  public class CategoryService: ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryVM>> GetAllCategories()
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
    }
   
}
