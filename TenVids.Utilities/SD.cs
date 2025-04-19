using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenVids.Utilities
{
   public static class SD
    {

        public const string AdminRole = "Admin";
        public const string ModeratorRole = "Moderator";
        public const string UserRole = "User";
        public static readonly List<string> Roles = new List<string> { AdminRole, ModeratorRole, UserRole };
        public const int fileSizeLimit = 10485760; // 10 MB
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
    }
}
