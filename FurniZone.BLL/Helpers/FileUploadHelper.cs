using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FurniZone.BLL.Helpers
{
    public interface IFileUploadHelper
    {
        Task<string> UploadImageAsync(IFormFile image, string folder = "products");
        void DeleteImage(string imagePath);
        bool IsValidImage(IFormFile file);
    }

    public class FileUploadHelper : IFileUploadHelper
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadImageAsync(IFormFile image, string folder = "products")
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Invalid image file");

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException($"File extension {extension} is not allowed");

            if (image.Length > MaxFileSize)
                throw new ArgumentException("File size exceeds 5MB limit");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", folder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/images/{folder}/{fileName}";
        }

        public void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            var filePath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension) && file.Length <= MaxFileSize;
        }
    }
}
