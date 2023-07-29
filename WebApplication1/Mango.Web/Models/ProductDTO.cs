﻿using Mango.Web.Utility;
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        [Range(1,100)]
        public int Count { get; set; } = 1;
        [MaxFileSize(100)]
        [AllowedExtensions(new string[] {".jpeg",".jpg",".png",".webp" })]
        public IFormFile? Image { get; set; }
    }
}
