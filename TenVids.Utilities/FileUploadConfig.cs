
namespace TenVids.Utilities
{
    public class FileUploadConfig
    {
        public int ImageMaxSizeInMB { get; set; }
        public string[] ImageContentTypes { get; set; }
        public int VideoMaxSizeInMB { get; set; }
        public string[] VideoContentTypes { get; set; }
    }
}
