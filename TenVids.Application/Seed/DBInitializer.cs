using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Utilities;
using TenVids.Utilities.FileHelpers;

namespace TenVids.Seed
{
    public static class DBInitializer
    {
        public static async Task InitializeAsync(
     TenVidsApplicationContext context,
     UserManager<ApplicationUser> userManager,
     RoleManager<AppRole> roleManager,
     IPicService pictureService,IWebHostEnvironment webHostInvironment)
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
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@tenvids.com",
                    Name = "Admin",
                };

                await userManager.CreateAsync(admin, "@Admin12345");
                await userManager.AddToRolesAsync(admin, new[] { SD.AdminRole, SD.UserRole, SD.ModeratorRole });

                // Add GivenName Claim to User
                await userManager.AddClaimAsync(admin, new Claim(ClaimTypes.GivenName, "Admin"));

                var user = new ApplicationUser
                {
                    UserName = "user",
                    Email = "user@tenvids.com",
                    Name = "User",
                };

                await userManager.CreateAsync(user, "@User12345");
                await userManager.AddToRoleAsync(user, SD.UserRole);

                // Add GivenName Claim to User
                await userManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, "User"));

                var tensae = new ApplicationUser
                {
                    UserName = "tensae",
                    Email = "tensae@tenvids.com",
                    Name = "Tensae",
                };

                await userManager.CreateAsync(tensae, "@Ten12345");
                await userManager.AddToRoleAsync(tensae, SD.UserRole);

                // Add GivenName Claim to User
                await userManager.AddClaimAsync(tensae, new Claim(ClaimTypes.GivenName, "Tensae"));

                var tensaeschannel = new Channel
                {
                    Name = "Tensae's",
                    Description = "Tensae's Entertainment, Ethiopian music channel",
                    AppUserId = tensae.Id,
                };

                await context.Channels.AddAsync(tensaeschannel);

                var dino = new ApplicationUser
                {
                    UserName = "Dino",
                    Email = "dino@tenvids.com",
                    Name = "Dino",
                };

                await userManager.CreateAsync(dino, "@Dino12345");
                await userManager.AddToRoleAsync(dino, SD.UserRole);

                // Add GivenName Claim to User
                await userManager.AddClaimAsync(dino, new Claim(ClaimTypes.GivenName, "Dino"));

                var dinoschannel = new Channel
                {
                    Name = "Dino's",
                    Description = "Dino's Entertainment, R&B music channel",
                    AppUserId = dino.Id,
                };

                await context.Channels.AddAsync(dinoschannel);


                var moderator = new ApplicationUser
                {
                    UserName = "moderator",
                    Email = "moderator@tenvids.com",
                    Name = "Moderator",
                };

                await userManager.CreateAsync(moderator, "@Moderator12345");
                await userManager.AddToRoleAsync(moderator, SD.ModeratorRole);

                var animal = new Category { Name = "Animal" };
                var food = new Category { Name = "Food" };
                var game = new Category { Name = "Game" };
                var nature = new Category { Name = "Nature" };
                var news = new Category { Name = "News" };
                var sport = new Category { Name = "Sport" };

                await context.Categories.AddRangeAsync(new[] { animal, food, game, nature, news, sport });
                await context.SaveChangesAsync();
                var folderpath = Path.Combine(webHostInvironment.WebRootPath, "image");
                if (Directory.Exists(folderpath))
                {
                    Directory.Delete(folderpath,true);
                }
                var imagedir = new System.IO.DirectoryInfo("Seed/Files/Thumbnails");
                var videodir = new System.IO.DirectoryInfo("Seed/Files/Videos");

                System.IO.FileInfo[] imageFiles = imagedir.GetFiles();
                System.IO.FileInfo[] videoFiles = videodir.GetFiles();

                var description = "This is a sample video description. It can be used to provide information about the video content, its purpose, and any other relevant details that viewers might find interesting or helpful.";

                for (int i = 0; i < 30 && i < videoFiles.Length; i++)
                {
                    var allNames = videoFiles[i].Name.Split('-');
                    var categoryName = allNames[0];
                    var title = allNames[2].Split('.')[0];
                    var categoryid = await context.Categories
                        .Where(c => c.Name.ToLower() == categoryName.ToLower())
                        .Select(c => c.Id)
                        .FirstOrDefaultAsync();

                    IFormFile imageFile = ConvertToFile(imageFiles[i]);
                    IFormFile videoFile = ConvertToFile(videoFiles[i]);

                    var videotoadd = new Videos
                    {
                        Title = title,
                        Description = description,
                        VideoFile = new VideoFiles
                        {

                            ContentType = SD.GetContentType( videoFiles[i].Extension),
                            Contents = ConvertToByteArray(videoFile).GetAwaiter().GetResult(),
                            Extension = videoFiles[i].Extension,
                        },
                        Thumbnail = pictureService.UploadPics(imageFile),
                        CategoryId = categoryid,
                        ChannelId = (i % 2 == 0) ? tensaeschannel.Id : dinoschannel.Id,
                        CreatedAt = SD.GetRandomDate(new System.DateTime(2023, 1, 1), DateTime.UtcNow, i),
                    };
                    context.Videos.Add(videotoadd);

                    await context.SaveChangesAsync();
                }

            }
        }

        #region Helper Methods
        private static IFormFile ConvertToFile(System.IO.FileInfo fileInfo)
        {
            var stream = new System.IO.FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            IFormFile formFile = new FormFile(stream, 0, stream.Length, fileInfo.Name, fileInfo.Name);
            return formFile;
        }

        private static async Task<byte[]> ConvertToByteArray(IFormFile file)
        {
            byte[] contents;
            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);
            contents = memoryStream.ToArray();
            return contents;

        }

        #endregion
    }
}