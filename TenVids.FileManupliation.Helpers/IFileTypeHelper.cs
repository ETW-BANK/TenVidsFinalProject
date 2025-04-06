

using Microsoft.AspNetCore.Mvc.Rendering;

namespace TenVids.FileManupliation.Helpers
{
   public interface IFileTypeHelper
    {
        string[] AcceptableContentTypes(string type);

       
    }
}
