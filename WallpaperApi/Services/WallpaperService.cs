using Microsoft.EntityFrameworkCore;
using WallpaperApi.Data;
using WallpaperApi.DTOs;
using WallpaperApi.Models;

namespace WallpaperApi.Services
{
    public interface IWallpaperService
    {
        Task<WallpaperDto> UploadWallpaperAsync(int userId, UploadWallpaperDto uploadDto, string imageUrl, string thumbnailUrl, int width, int height, long fileSize);
        Task<WallpaperDto> GetWallpaperByIdAsync(int wallpaperId);
        Task<IEnumerable<WallpaperListDto>> GetWallpapersAsync(int page = 1, int pageSize = 20, string? category = null, bool onlyApproved = true);
        Task<WallpaperDto> UpdateWallpaperAsync(int userId, int wallpaperId, UpdateWallpaperDto updateDto);
        Task<bool> DeleteWallpaperAsync(int userId, int wallpaperId, bool isAdmin);
        Task<WallpaperDto> ApproveWallpaperAsync(int adminUserId, int wallpaperId, ApproveWallpaperDto approveDto);
        Task<IEnumerable<WallpaperListDto>> GetPendingWallpapersAsync();
    }

    public class WallpaperService : IWallpaperService
    {
        private readonly WallpaperDbContext _context;

        public WallpaperService(WallpaperDbContext context)
        {
            _context = context;
        }

        public async Task<WallpaperDto> UploadWallpaperAsync(int userId, UploadWallpaperDto uploadDto, 
            string imageUrl, string thumbnailUrl, int width, int height, long fileSize)
        {
            var wallpaper = new Wallpaper
            {
                Title = uploadDto.Title,
                Description = uploadDto.Description,
                Category = uploadDto.Category,
                Tags = uploadDto.Tags,
                ImageUrl = imageUrl,
                ThumbnailUrl = thumbnailUrl,
                Width = width,
                Height = height,
                FileSize = fileSize,
                UploadedByUserId = userId,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Wallpapers.Add(wallpaper);
            await _context.SaveChangesAsync();

            // Load the user for the response
            await _context.Entry(wallpaper).Reference(w => w.UploadedBy).LoadAsync();

            return MapToWallpaperDto(wallpaper);
        }

        public async Task<WallpaperDto> GetWallpaperByIdAsync(int wallpaperId)
        {
            var wallpaper = await _context.Wallpapers
                .Include(w => w.UploadedBy)
                .FirstOrDefaultAsync(w => w.Id == wallpaperId);

            if (wallpaper == null)
            {
                throw new Exception("Wallpaper not found");
            }

            // Increment view count
            wallpaper.ViewCount++;
            await _context.SaveChangesAsync();

            var commentCount = await _context.Comments.CountAsync(c => c.WallpaperId == wallpaperId);

            return MapToWallpaperDto(wallpaper, commentCount);
        }

        public async Task<IEnumerable<WallpaperListDto>> GetWallpapersAsync(int page = 1, int pageSize = 20, 
            string? category = null, bool onlyApproved = true)
        {
            var query = _context.Wallpapers.AsQueryable();

            if (onlyApproved)
            {
                query = query.Where(w => w.IsApproved);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(w => w.Category == category);
            }

            var wallpapers = await query
                .OrderByDescending(w => w.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WallpaperListDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    ThumbnailUrl = w.ThumbnailUrl,
                    Category = w.Category,
                    LikeCount = w.LikeCount,
                    ViewCount = w.ViewCount,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();

            return wallpapers;
        }

        public async Task<WallpaperDto> UpdateWallpaperAsync(int userId, int wallpaperId, UpdateWallpaperDto updateDto)
        {
            var wallpaper = await _context.Wallpapers
                .Include(w => w.UploadedBy)
                .FirstOrDefaultAsync(w => w.Id == wallpaperId);

            if (wallpaper == null)
            {
                throw new Exception("Wallpaper not found");
            }

            if (wallpaper.UploadedByUserId != userId)
            {
                throw new Exception("Unauthorized");
            }

            if (updateDto.Title != null)
            {
                wallpaper.Title = updateDto.Title;
            }

            if (updateDto.Description != null)
            {
                wallpaper.Description = updateDto.Description;
            }

            if (updateDto.Category != null)
            {
                wallpaper.Category = updateDto.Category;
            }

            if (updateDto.Tags != null)
            {
                wallpaper.Tags = updateDto.Tags;
            }

            wallpaper.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var commentCount = await _context.Comments.CountAsync(c => c.WallpaperId == wallpaperId);

            return MapToWallpaperDto(wallpaper, commentCount);
        }

        public async Task<bool> DeleteWallpaperAsync(int userId, int wallpaperId, bool isAdmin)
        {
            var wallpaper = await _context.Wallpapers.FindAsync(wallpaperId);

            if (wallpaper == null)
            {
                throw new Exception("Wallpaper not found");
            }

            if (!isAdmin && wallpaper.UploadedByUserId != userId)
            {
                throw new Exception("Unauthorized");
            }

            _context.Wallpapers.Remove(wallpaper);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<WallpaperDto> ApproveWallpaperAsync(int adminUserId, int wallpaperId, ApproveWallpaperDto approveDto)
        {
            var wallpaper = await _context.Wallpapers
                .Include(w => w.UploadedBy)
                .FirstOrDefaultAsync(w => w.Id == wallpaperId);

            if (wallpaper == null)
            {
                throw new Exception("Wallpaper not found");
            }

            if (approveDto.Approve)
            {
                wallpaper.IsApproved = true;
                wallpaper.IsRejected = false;
                wallpaper.RejectionReason = null;
                wallpaper.ApprovedByUserId = adminUserId;
            }
            else
            {
                wallpaper.IsApproved = false;
                wallpaper.IsRejected = true;
                wallpaper.RejectionReason = approveDto.RejectionReason;
            }

            wallpaper.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var commentCount = await _context.Comments.CountAsync(c => c.WallpaperId == wallpaperId);

            return MapToWallpaperDto(wallpaper, commentCount);
        }

        public async Task<IEnumerable<WallpaperListDto>> GetPendingWallpapersAsync()
        {
            var wallpapers = await _context.Wallpapers
                .Where(w => !w.IsApproved && !w.IsRejected)
                .OrderByDescending(w => w.CreatedAt)
                .Select(w => new WallpaperListDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    ThumbnailUrl = w.ThumbnailUrl,
                    Category = w.Category,
                    LikeCount = w.LikeCount,
                    ViewCount = w.ViewCount,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();

            return wallpapers;
        }

        private WallpaperDto MapToWallpaperDto(Wallpaper wallpaper, int commentCount = 0)
        {
            return new WallpaperDto
            {
                Id = wallpaper.Id,
                Title = wallpaper.Title,
                Description = wallpaper.Description,
                ImageUrl = wallpaper.ImageUrl,
                ThumbnailUrl = wallpaper.ThumbnailUrl,
                Category = wallpaper.Category,
                Tags = wallpaper.Tags,
                Width = wallpaper.Width,
                Height = wallpaper.Height,
                FileSize = wallpaper.FileSize,
                UploadedBy = new UserDto
                {
                    Id = wallpaper.UploadedBy.Id,
                    Username = wallpaper.UploadedBy.Username,
                    DisplayName = wallpaper.UploadedBy.DisplayName,
                    AvatarUrl = wallpaper.UploadedBy.AvatarUrl,
                    Email = wallpaper.UploadedBy.Email,
                    Bio = wallpaper.UploadedBy.Bio,
                    IsAdmin = wallpaper.UploadedBy.IsAdmin,
                    CreatedAt = wallpaper.UploadedBy.CreatedAt
                },
                IsApproved = wallpaper.IsApproved,
                CreatedAt = wallpaper.CreatedAt,
                ViewCount = wallpaper.ViewCount,
                LikeCount = wallpaper.LikeCount,
                FavoriteCount = wallpaper.FavoriteCount,
                CommentCount = commentCount
            };
        }
    }
}
