using Microsoft.Extensions.Configuration;


namespace TenVids.FileHeper
{
    public static class FileTypeHelper
    {
        private static IConfiguration? Configeration;

        public static void Initialize(IConfiguration configuration)
        {
            Configeration = configuration;
        }

        public static string[] AcceptableContentTypes(string type)
        {
            if (type.Equals("image"))
            {
                return Configeration?.GetSection("FileUpload:ImageContentTypes").g

            }
            else
            {
                return Configeration?.GetSection("FileUpload:VideoContentTypes")
                    .Get<string[]>()!;

            }
        }
    }
}

