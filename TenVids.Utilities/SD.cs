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
    }
}
