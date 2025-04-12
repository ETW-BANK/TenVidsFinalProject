
namespace TenVids.Utilities
{
   public static class SD
    {

        public const string AdminRole = "Admin";
        public const string ModeratorRole = "Moderator";
        public const string UserRole = "User";
        public static readonly List<string> Roles = new List<string> { AdminRole, ModeratorRole, UserRole };
        public const int fileSizeLimit = 10485760; // 10 MB

        public static DateTime GetRandomDays(DateTime minDate, DateTime maxDate, int seed)
        {
            Random random = new Random(seed);
            int range = (maxDate - minDate).Days;
            return minDate.AddDays(random.Next(range + 1));
        }

    }

  
}
