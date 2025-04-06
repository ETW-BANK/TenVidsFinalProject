

using Microsoft.AspNetCore.Mvc.Rendering;

namespace TenVids.FileManupliation.Helpers
{
   public interface IFileTypeHelper
    {
        IEnumerable<string> AcceptableContentTypes(string type);

        bool IsAcceptableContentType(string type, string contentType);


    }
}
