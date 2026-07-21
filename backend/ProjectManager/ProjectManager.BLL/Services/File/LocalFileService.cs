using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Services.File
{
    public class LocalFileService : ILocalFileService
    {
        private readonly string _uploadsFolder;

        public LocalFileService()
        {
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var physicalPath = Path.Combine(_uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{uniqueFileName}";
        }

        public async Task<(byte[] Bytes, string ContentType)?> GetFileAsync(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            var cleanPath = relativePath.TrimStart('/', '\\');
            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanPath);

            if (System.IO.File.Exists(physicalPath))
                return null;

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(physicalPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(physicalPath);
            return (bytes, contentType);
        }

        public bool DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return false;

            var cleanPath = relativePath.TrimStart('/', '\\');
            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanPath);

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
                return true;
            }

            return false;
        }
    }
}
