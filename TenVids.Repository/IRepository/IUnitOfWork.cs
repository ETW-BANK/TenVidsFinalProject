using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TenVids.Repository.IRepository
{
    public interface IUnitOfWork: IDisposable
    {
        IChannelRepository Channel { get; }
        
       Task <bool> CompleteAsync();
    }
}
