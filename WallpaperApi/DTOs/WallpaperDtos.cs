using System.ComponentModel.DataAnnotations;

namespace WallpaperApi.DTOs
{
    public class UploadWallpaperDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public string? Tags { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;
    }

    public class UpdateWallpaperDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public string? Tags { get; set; }
    }

    public class ApproveWallpaperDto
    {
        [Required]
        public bool Approve { get; set; }

        public string? RejectionReason { get; set; }
    }

    public class WallpaperDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long FileSize { get; set; }
        public UserDto UploadedBy { get; set; } = null!;
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int FavoriteCount { get; set; }
        public int CommentCount { get; set; }
    }

    public class WallpaperListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int LikeCount { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
