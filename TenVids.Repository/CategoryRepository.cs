
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

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var categoryFromDb = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == category.Id);

            if (categoryFromDb == null) return null;

            categoryFromDb.Name = category.Name;
            await _context.SaveChangesAsync();

            return categoryFromDb;
        }
    }
}
