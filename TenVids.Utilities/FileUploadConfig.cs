using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
