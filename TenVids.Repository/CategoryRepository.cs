
using Microsoft.EntityFrameworkCore;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly TenVidsApplicationContext _context;

        public CategoryRepository(TenVidsApplicationContext context) : base(context)
        {
            _context = context;
        }

        
    }
}
