using Microsoft.EntityFrameworkCore;
using WallpaperApi.Models;

namespace WallpaperApi.Data
{
    public class WallpaperDbContext : DbContext
    {
        public WallpaperDbContext(DbContextOptions<WallpaperDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallpaper> Wallpapers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User table configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Wallpaper table configuration
            modelBuilder.Entity<Wallpaper>(entity =>
            {
                entity.HasOne(w => w.UploadedBy)
                    .WithMany(u => u.UploadedWallpapers)
                    .HasForeignKey(w => w.UploadedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Comment table configuration
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Wallpaper)
                    .WithMany(w => w.Comments)
                    .HasForeignKey(c => c.WallpaperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Like table configuration
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.WallpaperId }).IsUnique();

                entity.HasOne(l => l.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.Wallpaper)
                    .WithMany(w => w.Likes)
                    .HasForeignKey(l => l.WallpaperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Favorite table configuration
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.WallpaperId }).IsUnique();

                entity.HasOne(f => f.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Wallpaper)
                    .WithMany(w => w.Favorites)
                    .HasForeignKey(f => f.WallpaperId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
