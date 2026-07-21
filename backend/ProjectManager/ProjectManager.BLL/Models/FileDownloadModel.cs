using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.Models
{
    public class FileDownloadModel
    {
        public byte[] Bytes { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}
