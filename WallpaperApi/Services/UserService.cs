using Microsoft.EntityFrameworkCore;
using WallpaperApi.Data;
using WallpaperApi.DTOs;
using WallpaperApi.Models;

namespace WallpaperApi.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<IEnumerable<WallpaperListDto>> GetUserWallpapersAsync(int userId);
        Task<IEnumerable<WallpaperListDto>> GetUserFavoritesAsync(int userId);
        Task<IEnumerable<CommentDto>> GetUserCommentsAsync(int userId);
    }

    public class UserService : IUserService
    {
        private readonly WallpaperDbContext _context;

        public UserService(WallpaperDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (updateProfileDto.DisplayName != null)
            {
                user.DisplayName = updateProfileDto.DisplayName;
            }

            if (updateProfileDto.Bio != null)
            {
                user.Bio = updateProfileDto.Bio;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                throw new Exception("Current password is incorrect");
            }

            // Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<WallpaperListDto>> GetUserWallpapersAsync(int userId)
        {
            var wallpapers = await _context.Wallpapers
                .Where(w => w.UploadedByUserId == userId)
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

        public async Task<IEnumerable<WallpaperListDto>> GetUserFavoritesAsync(int userId)
        {
            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Wallpaper)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new WallpaperListDto
                {
                    Id = f.Wallpaper.Id,
                    Title = f.Wallpaper.Title,
                    ThumbnailUrl = f.Wallpaper.ThumbnailUrl,
                    Category = f.Wallpaper.Category,
                    LikeCount = f.Wallpaper.LikeCount,
                    ViewCount = f.Wallpaper.ViewCount,
                    CreatedAt = f.Wallpaper.CreatedAt
                })
                .ToListAsync();

            return favorites;
        }

        public async Task<IEnumerable<CommentDto>> GetUserCommentsAsync(int userId)
        {
            var comments = await _context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    User = new UserDto
                    {
                        Id = c.User.Id,
                        Username = c.User.Username,
                        DisplayName = c.User.DisplayName,
                        AvatarUrl = c.User.AvatarUrl,
                        Email = c.User.Email,
                        Bio = c.User.Bio,
                        IsAdmin = c.User.IsAdmin,
                        CreatedAt = c.User.CreatedAt
                    },
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return comments;
        }
    }
}
