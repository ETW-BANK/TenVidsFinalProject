
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
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

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
