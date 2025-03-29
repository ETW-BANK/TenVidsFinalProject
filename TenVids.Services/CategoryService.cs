
using TenVids.Repository.IRepository;
using TenVids.Services.IServices;

namespace TenVids.Services
{
  public class CategoryService: ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
   
}
