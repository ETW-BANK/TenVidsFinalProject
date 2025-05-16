using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TenVids.Data.Access.Data;
using TenVids.Data.Access.IRepo;


namespace TenVids.Repository
{
    public class UnitOfWork: IUnitOfWork
    {
 
        private readonly TenVidsApplicationContext _context;
        private readonly IConfiguration _configuration;
        public UnitOfWork(TenVidsApplicationContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }
        public IChannelRepository ChannelRepository => new ChannelRepository(_context); 
        public IVideosRepository VideosRepository => new VideosRepository(_context);
        public IVideoFileRepository VideoFileRepository => new VideoFileRepository(_context);   
        public ICategoryRepository CategoryRepository => new CategoryRepository(_context);
        public ICommentsRepository CommentsRepository => new CommentsRepository(_context);
        public IVideoViewRepository VideoViewRepository => new VideoViewRepository(_context,_configuration);
        public ILikesRepository LikesRepository => new LikesRepository(_context);   

        public async Task<bool> CompleteAsync()
        {
            try
            {
               
                int changes = await _context.SaveChangesAsync();
                return true; 
            }
            catch (DbUpdateException)
            {
               
                return false;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
  
}
