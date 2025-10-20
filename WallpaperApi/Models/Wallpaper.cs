using System.ComponentModel.DataAnnotations;

namespace WallpaperApi.Models
{
    public class Wallpaper
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Category { get; set; }

        public string? Tags { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public long FileSize { get; set; }

        [Required]
        public int UploadedByUserId { get; set; }

        public User UploadedBy { get; set; } = null!;

        public bool IsApproved { get; set; } = false;

        public bool IsRejected { get; set; } = false;

        public string? RejectionReason { get; set; }

        public int? ApprovedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int ViewCount { get; set; } = 0;

        public int LikeCount { get; set; } = 0;

        public int FavoriteCount { get; set; } = 0;

        // Navigation properties
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
