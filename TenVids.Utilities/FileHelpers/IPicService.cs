using Microsoft.AspNetCore.Http;


namespace TenVids.Utilities.FileHelpers
{
   public interface IPicService
    {

        string UploadPics(IFormFile file, string oldpath = "");
    }
}
