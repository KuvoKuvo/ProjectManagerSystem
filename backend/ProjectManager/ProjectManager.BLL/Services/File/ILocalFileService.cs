using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Services.File
{
    public interface ILocalFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task<(byte[] Bytes, string ContentType)?> GetFileAsync(string relativePath);
        bool DeleteFile(string relativePath);
    }
}
