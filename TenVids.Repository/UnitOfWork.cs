
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
            bool success = false;  
            if(_context.ChangeTracker.HasChanges())
            {
                success = await _context.SaveChangesAsync()>0;
            }
            return success;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
  
}
