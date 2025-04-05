
using Microsoft.EntityFrameworkCore;
using TenVids.Data.Access.Data;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class UnitOfWork: IUnitOfWork
    {
 
        private readonly TenVidsApplicationContext _context;
        public UnitOfWork(TenVidsApplicationContext context)
        {
            _context = context;
        
        }
        public IChannelRepository ChannelRepository => new ChannelRepository(_context); 
        public IVideosRepository VideosRepository => new VideosRepository(_context);

      public ICategoryRepository CategoryRepository => new CategoryRepository(_context);
        public async Task<bool> CompleteAsync()
        {
            try
            {
                // Always attempt to save changes regardless of HasChanges()
                int changes = await _context.SaveChangesAsync();
                return true; // EF Core will throw if there are problems
            }
            catch (DbUpdateException)
            {
                // Log the exception if needed
                return false;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
  
}
