using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenVids.Repository.IRepository;
using TenVids.Services.IServices;

namespace TenVids.Services
{
    public class VideosService: IVideosService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VideosService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
   
}
