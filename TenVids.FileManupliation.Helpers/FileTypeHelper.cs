

using Microsoft.Extensions.Options;
using TenVids.Utilities;

namespace TenVids.FileManupliation.Helpers
{
    public class FileTypeHelper : IFileTypeHelper
    {
        private readonly FileUploadConfig _config;

        public FileTypeHelper(IOptions<FileUploadConfig> config)
        {
            _config = config.Value;
        }

        public IEnumerable<string> AcceptableContentTypes(string type)
        {
            return type?.ToLower() switch
            {
                "image" => _config.ImageContentTypes ?? Array.Empty<string>(),
                "video" => _config.VideoContentTypes ?? Array.Empty<string>(),
                _ => Array.Empty<string>()
            };
        }

        public bool IsAcceptableContentType(string type, string contentType)
        {
            if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(type))
                return false;

            var allowedTypes = AcceptableContentTypes(type);
            return allowedTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
        }
    }
}