using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Utilities;

namespace TenVids.Data.Access
{
    public static class DBInitializer
    {
        public static async Task InitializeAsync(
            TenVidsApplicationContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<AppRole> roleManager)
        {
         
            if (context.Database.GetPendingMigrations().Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

         
            if (!roleManager.Roles.Any())
            {
                foreach (var role in SD.Roles)
                {
                    await roleManager.CreateAsync(new AppRole { Name = role });
                }
            }

          
            if (!userManager.Users.Any())
            {
                // Create admin user
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@tenvids.com",
                    Name = "Admin",
                   
                };

                await userManager.CreateAsync(admin, "@Admin12345");
                await userManager.AddToRolesAsync(admin, new[] { SD.AdminRole, SD.UserRole, SD.ModeratorRole });

               
                var user = new ApplicationUser
                {
                    UserName = "user",
                    Email = "user@tenvids.com",
                    Name = "User",
                 
                };

                await userManager.CreateAsync(user, "@User12345");
                await userManager.AddToRoleAsync(user, SD.UserRole);

               
                var moderator = new ApplicationUser
                {
                    UserName = "moderator",
                    Email = "moderator@tenvids.com",
                    Name = "Moderator",
                  
                };

                await userManager.CreateAsync(moderator, "@Moderator12345");
                await userManager.AddToRoleAsync(moderator, SD.ModeratorRole);
            }
        }
    }
}