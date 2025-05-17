using Tensae.Generic.Repository;
using TenVids.Data.Access.Data;
using TenVids.Data.Access.IRepo;
using TenVids.Models;


namespace TenVids.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(TenVidsApplicationContext context) : base(context)
        {
          
        }

    }
}
