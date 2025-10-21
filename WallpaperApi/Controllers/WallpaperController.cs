using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WallpaperApi.DTOs;
using WallpaperApi.Services;

namespace WallpaperApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WallpaperController : ControllerBase
    {
        private readonly IWallpaperService _wallpaperService;
        private readonly IFileUploadService _fileUploadService;

        public WallpaperController(IWallpaperService wallpaperService, IFileUploadService fileUploadService)
        {
            _wallpaperService = wallpaperService;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWallpapers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? category = null)
        {
            try
            {
                var wallpapers = await _wallpaperService.GetWallpapersAsync(page, pageSize, category);
                return Ok(wallpapers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{wallpaperId}")]
        public async Task<IActionResult> GetWallpaper(int wallpaperId)
        {
            try
            {
                var wallpaper = await _wallpaperService.GetWallpaperByIdAsync(wallpaperId);
                return Ok(wallpaper);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadWallpaper([FromForm] UploadWallpaperDto uploadDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Upload image
                var (imageUrl, thumbnailUrl, width, height, fileSize) = 
                    await _fileUploadService.UploadImageAsync(uploadDto.Image);

                // Create wallpaper record
                var wallpaper = await _wallpaperService.UploadWallpaperAsync(
                    userId, uploadDto, imageUrl, thumbnailUrl, width, height, fileSize);

                return Ok(wallpaper);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{wallpaperId}")]
        public async Task<IActionResult> UpdateWallpaper(int wallpaperId, [FromBody] UpdateWallpaperDto updateDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var wallpaper = await _wallpaperService.UpdateWallpaperAsync(userId, wallpaperId, updateDto);
                return Ok(wallpaper);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{wallpaperId}")]
        public async Task<IActionResult> DeleteWallpaper(int wallpaperId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var isAdmin = User.IsInRole("Admin");
                await _wallpaperService.DeleteWallpaperAsync(userId, wallpaperId, isAdmin);
                return Ok(new { message = "Wallpaper deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingWallpapers()
        {
            try
            {
                var wallpapers = await _wallpaperService.GetPendingWallpapersAsync();
                return Ok(wallpapers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{wallpaperId}/approve")]
        public async Task<IActionResult> ApproveWallpaper(int wallpaperId, [FromBody] ApproveWallpaperDto approveDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var wallpaper = await _wallpaperService.ApproveWallpaperAsync(userId, wallpaperId, approveDto);
                return Ok(wallpaper);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
