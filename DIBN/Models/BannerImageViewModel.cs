using Microsoft.AspNetCore.Http;

namespace DIBN.Models
{
    public class BannerImageViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public byte[] PictureBinary { get; set; }
        public string Path { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedOnUtc { get; set; }
        public string ModifyOnUtc { get; set; }
        public IFormFile formFile { get; set; }
        public string Module { get; set; }
    }
}
