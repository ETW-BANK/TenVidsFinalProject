
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
       
        public CategoryRepository(TenVidsApplicationContext context) : base(context)
        {
          
        }

        
    }
}
