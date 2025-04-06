
using Microsoft.Extensions.Configuration;

namespace TenVids.FileManupliation.Helpers
{
    public class FileTypeHelper:IFileTypeHelper
    {
        private readonly IConfiguration Configeration;
        
        public FileTypeHelper(IConfiguration config)
        {
            Configeration = config;
        }

        public string[] AcceptableContentTypes(string type)
        {
           if(type.Equals("image"))
            {
                return Configeration.GetSection("FileUpload:ImageContentTypes").Get<string[]>()!;

            }
            else
            {
                return Configeration.GetSection("FileUpload:VideoContentTypes").Get<string[]>()!;

            }
        }

      
    }
}
