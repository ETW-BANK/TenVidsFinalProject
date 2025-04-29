using Microsoft.EntityFrameworkCore;
using TenVids.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TenVids.Data.Access.Data
{
    public class TenVidsApplicationContext:IdentityDbContext<ApplicationUser,AppRole,string>
    {
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Videos> Videos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<VideoViews> VideoViews { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Subscribe> Subscribe { get; set; }
        public TenVidsApplicationContext(DbContextOptions<TenVidsApplicationContext> option): base(option)    
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
         .HasKey(c => new { c.Id }); 

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.AppUser)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Video)
                .WithMany(v => v.Comments)
                .HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subscribe>()
                .HasKey(s => new { s.AppUserId, s.ChannelId });

            modelBuilder.Entity<Subscribe>()
               .HasOne(c => c.AppUser)
               .WithMany(u => u.Subscribtions)
               .HasForeignKey(c => c.AppUserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscribe>()
                .HasOne(c => c.Channel)
                .WithMany(v => v.Subscribers)
                .HasForeignKey(c => c.ChannelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Likes>()
              .HasKey(s => new { s.AppUserId, s.VideoId });

            modelBuilder.Entity<Likes>()
               .HasOne(c => c.AppUser)
               .WithMany(u => u.Likes)
               .HasForeignKey(c => c.AppUserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Likes>()
                .HasOne(c => c.Video)
                .WithMany(v => v.Likes)
                .HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VideoViews>()
                .HasKey(v => new { v.Id});
            modelBuilder.Entity<VideoViews>()
                .HasOne(v => v.AppUser)
                .WithMany(u => u.VideoViews)
                .HasForeignKey(v => v.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VideoViews>()
                .HasOne(v => v.Video)
                .WithMany(v => v.VideoViewers)
                .HasForeignKey(v => v.VideoId)
                .OnDelete(DeleteBehavior.Restrict);

       

        }

    }
}
