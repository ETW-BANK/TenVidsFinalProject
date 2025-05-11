using Microsoft.AspNetCore.Mvc.Rendering;


namespace TenVids.Utilities
{
   public static class SD
    {

        public const string AdminRole = "Admin";
        public const string ModeratorRole = "Moderator";
        public const string UserRole = "User";
        public static readonly List<string> Roles = new List<string> { AdminRole, ModeratorRole, UserRole };
        public const int fileSizeLimit = 10485760; // 10 MB
        public static readonly List<string> LocalIpAddresses =new List<string> { "127.0.0.1", "::1" };

        public static string NormalizeIp(string ipAddress)
        {
            return LocalIpAddresses.Contains(ipAddress) ? "127.0.0.1" : ipAddress;
        }
        public static DateTime GetRandomDate(DateTime minDate, DateTime maxDate, int seed)
        {
            Random random = new Random(seed);
            int range = (maxDate - minDate).Days;
            return minDate.AddDays(random.Next(range + 1));
        }
        public static int GetRandomNumber(int min, int max, int seed)
        {
            Random random = new Random(seed);
            return random.Next(min, max);
        }  

        public static string IsActive(this IHtmlHelper htmlHelper,string controller, string action, string cssClass="active")
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();
            var returnActive=controller== routeController && action == routeAction ? cssClass : string.Empty;

            return returnActive;  
        }
        
        public static string IsActivePage(this IHtmlHelper htmlHelper, string page)
        {
            var currentPage = htmlHelper.ViewContext.HttpContext.Request.Query["page"].ToString(); 
            var isPageMatch=string.IsNullOrEmpty(page)||currentPage == page;

            if(string.IsNullOrEmpty(currentPage) && page=="Home") 
            {
                return "active";
            }
            return isPageMatch ? "active" : string.Empty;
        }

        public static string GetContentType(string fileextension)
        {
            return fileextension switch
            {

                ".mp4" => "video/mp4",
                ".mov"=> "video/quicktime",
                ".avi"=> "video/x-msvideo",
                ".mwv"=> "video/x-ms-wmv",
                ".flv"=>"video/x-flv",
                ".mkv"=> "video/x-matroska",
                ".webm" => "video/webm",
                ".ogv" => "video/ogg",
                ".3gp" => "video/3gpp",
                ".3g2" => "video/3gpp2",
                _ => "video/mp4" // Default case

            };
        }
        public static string GetExtension(string contentType)
        {
            return contentType switch
            {
                "video/mp4" => ".mp4",
                "video/quicktime"=>".mov",
                "video/x-msvideo"=>".avi",
                "video/x-ms-wmv" => ".wmv",
                "video/x-flv" => ".flv",
                "video/x-matroska" => ".mkv",
                "video/webm" => ".webm",
                "video/ogg" => ".ogv",
                "video/3gpp" => ".3gp",
                "video/3gpp2" => ".3g2",
               _ => ".mp4" // Default case   
            };
        }
        public static string FormatView(int views)
        {
            if (views >= 1000000)
            {
                return (views / 1000000).ToString() + "M";
            }
            else if (views >= 1000)
            {
                return (views / 1000).ToString() + "K";
            }
            else
            {
                return views.ToString();
            }
        }

        public static string TimeAgo(DateTime dateTime)
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan timeSpan = now - dateTime;

            double totalSeconds = Math.Floor(timeSpan.TotalSeconds);
            double minutes = Math.Floor(timeSpan.TotalMinutes);
            double hours = Math.Floor(timeSpan.TotalHours);
            double days = Math.Floor(timeSpan.TotalDays);
            double months = Math.Floor(days / 30);
            double years = Math.Floor(days / 365);

            if (totalSeconds < 60)
            {
                return "Just now";
            }
            else if (minutes < 60)
            {
                return $"{minutes} minute{(minutes != 1 ? "s" : "")} ago";
            }
            else if (hours < 24)
            {
                return $"{hours} hour{(hours != 1 ? "s" : "")} ago";
            }
            else if (days < 30)
            {
                return $"{days} day{(days != 1 ? "s" : "")} ago";
            }
            else if (days < 365)
            {
                return $"{months} month{(months != 1 ? "s" : "")} ago";
            }
            else
            {
                return $"{years} year{(years != 1 ? "s" : "")} ago";
            }
        }
    }
}
