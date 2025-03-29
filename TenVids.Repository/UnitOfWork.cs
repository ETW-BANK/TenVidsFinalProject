using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenVids.Data.Access.Data;
using TenVids.Repository.IRepository;

namespace TenVids.Repository
{
    public class UnitOfWork: IUnitOfWork
    {
        public IChannelRepository Channel { get; private set; }
        private readonly TenVidsApplicationContext _context;

        public UnitOfWork(TenVidsApplicationContext context)
        {
            _context = context;
            Channel = new ChannelRepository(_context);
        }

        public async Task<bool> CompleteAsync()
        {
            bool success = false;  
            if(_context.ChangeTracker.HasChanges())
            {
                success = await _context.SaveChangesAsync() > 0;
            }
            return success;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
  
}
