using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenVids.Models;

namespace TenVids.Utilities.FileHelpers
{
    public class PicService : IPicService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PicService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void DeletePhotoLocally(string photourl)
        {
            string wwwwRootPath = _webHostEnvironment.WebRootPath;
            string uploadPath = Path.Combine(wwwwRootPath, @"image\thumbnails");

            var oldFilePath = Path.Combine(wwwwRootPath, photourl.TrimStart('\\'));

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        public string UploadPics(IFormFile file, string oldpath = "")
        {
            string wwwwRootPath = _webHostEnvironment.WebRootPath;
            string uploadPath = Path.Combine(wwwwRootPath, @"image\thumbnails");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            string fileNmae = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            if (!string.IsNullOrEmpty(oldpath))
            {

                var oldFilePath = Path.Combine(wwwwRootPath, oldpath.TrimStart('\\'));

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            using var fileStream = new FileStream(Path.Combine(uploadPath, fileNmae), FileMode.Create);
                
             file.CopyTo(fileStream);
                
                return @"\image\thumbnails\" + fileNmae;
         

        }
    }
}
