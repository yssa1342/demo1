using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WallpaperApi.DTOs;
using WallpaperApi.Services;

namespace WallpaperApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InteractionController : ControllerBase
    {
        private readonly IInteractionService _interactionService;

        public InteractionController(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        [HttpPost("wallpapers/{wallpaperId}/like")]
        public async Task<IActionResult> LikeWallpaper(int wallpaperId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _interactionService.LikeWallpaperAsync(userId, wallpaperId);
                return Ok(new { message = "Wallpaper liked successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("wallpapers/{wallpaperId}/like")]
        public async Task<IActionResult> UnlikeWallpaper(int wallpaperId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _interactionService.UnlikeWallpaperAsync(userId, wallpaperId);
                return Ok(new { message = "Wallpaper unliked successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("wallpapers/{wallpaperId}/like")]
        public async Task<IActionResult> IsWallpaperLiked(int wallpaperId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var isLiked = await _interactionService.IsWallpaperLikedAsync(userId, wallpaperId);
                return Ok(new { isLiked });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("wallpapers/{wallpaperId}/favorite")]
        public async Task<IActionResult> FavoriteWallpaper(int wallpaperId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _interactionService.FavoriteWallpaperAsync(userId, wallpaperId);
                return Ok(new { message = "Wallpaper favorited successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("wallpapers/{wallpaperId}/favorite")]
        public async Task<IActionResult> UnfavoriteWallpaper(int wallpaperId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _interactionService.UnfavoriteWallpaperAsync(userId, wallpaperId);
                return Ok(new { message = "Wallpaper unfavorited successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("wallpapers/{wallpaperId}/favorite")]
        public async Task<IActionResult> IsWallpaperFavorited(int wallpaperId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var isFavorited = await _interactionService.IsWallpaperFavoritedAsync(userId, wallpaperId);
                return Ok(new { isFavorited });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("wallpapers/{wallpaperId}/comments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWallpaperComments(int wallpaperId)
        {
            try
            {
                var comments = await _interactionService.GetWallpaperCommentsAsync(wallpaperId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("wallpapers/{wallpaperId}/comments")]
        public async Task<IActionResult> CreateComment(int wallpaperId, [FromBody] CreateCommentDto createCommentDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var comment = await _interactionService.CreateCommentAsync(userId, wallpaperId, createCommentDto);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentDto updateCommentDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var comment = await _interactionService.UpdateCommentAsync(userId, commentId, updateCommentDto);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var isAdmin = User.IsInRole("Admin");
                await _interactionService.DeleteCommentAsync(userId, commentId, isAdmin);
                return Ok(new { message = "Comment deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
