using System.ComponentModel.DataAnnotations;

namespace WallpaperApi.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        public int WallpaperId { get; set; }

        public Wallpaper Wallpaper { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
