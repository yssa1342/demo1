using Microsoft.EntityFrameworkCore;
using WallpaperApi.Data;
using WallpaperApi.DTOs;
using WallpaperApi.Models;

namespace WallpaperApi.Services
{
    public interface IInteractionService
    {
        Task<bool> LikeWallpaperAsync(int userId, int wallpaperId);
        Task<bool> UnlikeWallpaperAsync(int userId, int wallpaperId);
        Task<bool> IsWallpaperLikedAsync(int userId, int wallpaperId);
        Task<bool> FavoriteWallpaperAsync(int userId, int wallpaperId);
        Task<bool> UnfavoriteWallpaperAsync(int userId, int wallpaperId);
        Task<bool> IsWallpaperFavoritedAsync(int userId, int wallpaperId);
        Task<CommentDto> CreateCommentAsync(int userId, int wallpaperId, CreateCommentDto createCommentDto);
        Task<CommentDto> UpdateCommentAsync(int userId, int commentId, UpdateCommentDto updateCommentDto);
        Task<bool> DeleteCommentAsync(int userId, int commentId, bool isAdmin);
        Task<IEnumerable<CommentDto>> GetWallpaperCommentsAsync(int wallpaperId);
    }

    public class InteractionService : IInteractionService
    {
        private readonly WallpaperDbContext _context;

        public InteractionService(WallpaperDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LikeWallpaperAsync(int userId, int wallpaperId)
        {
            // Check if already liked
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.WallpaperId == wallpaperId);

            if (existingLike != null)
            {
                return true; // Already liked
            }

            var like = new Like
            {
                UserId = userId,
                WallpaperId = wallpaperId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Likes.Add(like);

            // Update like count
            var wallpaper = await _context.Wallpapers.FindAsync(wallpaperId);
            if (wallpaper != null)
            {
                wallpaper.LikeCount++;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnlikeWallpaperAsync(int userId, int wallpaperId)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.WallpaperId == wallpaperId);

            if (like == null)
            {
                return false;
            }

            _context.Likes.Remove(like);

            // Update like count
            var wallpaper = await _context.Wallpapers.FindAsync(wallpaperId);
            if (wallpaper != null && wallpaper.LikeCount > 0)
            {
                wallpaper.LikeCount--;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsWallpaperLikedAsync(int userId, int wallpaperId)
        {
            return await _context.Likes
                .AnyAsync(l => l.UserId == userId && l.WallpaperId == wallpaperId);
        }

        public async Task<bool> FavoriteWallpaperAsync(int userId, int wallpaperId)
        {
            // Check if already favorited
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.WallpaperId == wallpaperId);

            if (existingFavorite != null)
            {
                return true; // Already favorited
            }

            var favorite = new Favorite
            {
                UserId = userId,
                WallpaperId = wallpaperId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Favorites.Add(favorite);

            // Update favorite count
            var wallpaper = await _context.Wallpapers.FindAsync(wallpaperId);
            if (wallpaper != null)
            {
                wallpaper.FavoriteCount++;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnfavoriteWallpaperAsync(int userId, int wallpaperId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.WallpaperId == wallpaperId);

            if (favorite == null)
            {
                return false;
            }

            _context.Favorites.Remove(favorite);

            // Update favorite count
            var wallpaper = await _context.Wallpapers.FindAsync(wallpaperId);
            if (wallpaper != null && wallpaper.FavoriteCount > 0)
            {
                wallpaper.FavoriteCount--;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsWallpaperFavoritedAsync(int userId, int wallpaperId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.WallpaperId == wallpaperId);
        }

        public async Task<CommentDto> CreateCommentAsync(int userId, int wallpaperId, CreateCommentDto createCommentDto)
        {
            var comment = new Comment
            {
                UserId = userId,
                WallpaperId = wallpaperId,
                Content = createCommentDto.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Load user for response
            await _context.Entry(comment).Reference(c => c.User).LoadAsync();

            return MapToCommentDto(comment);
        }

        public async Task<CommentDto> UpdateCommentAsync(int userId, int commentId, UpdateCommentDto updateCommentDto)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            if (comment.UserId != userId)
            {
                throw new Exception("Unauthorized");
            }

            comment.Content = updateCommentDto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToCommentDto(comment);
        }

        public async Task<bool> DeleteCommentAsync(int userId, int commentId, bool isAdmin)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            if (!isAdmin && comment.UserId != userId)
            {
                throw new Exception("Unauthorized");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CommentDto>> GetWallpaperCommentsAsync(int wallpaperId)
        {
            var comments = await _context.Comments
                .Where(c => c.WallpaperId == wallpaperId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(MapToCommentDto);
        }

        private CommentDto MapToCommentDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                User = new UserDto
                {
                    Id = comment.User.Id,
                    Username = comment.User.Username,
                    DisplayName = comment.User.DisplayName,
                    AvatarUrl = comment.User.AvatarUrl,
                    Email = comment.User.Email,
                    Bio = comment.User.Bio,
                    IsAdmin = comment.User.IsAdmin,
                    CreatedAt = comment.User.CreatedAt
                },
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }
    }
}
