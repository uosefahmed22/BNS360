using BNS360.Core.Enums;
using BNS360.Core.Models.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Models
{
    public class BusinessModel
    {
        public int Id { get; set; }
        public string BusinessNameArabic { get; set; }
        public string? BusinessNameEnglish { get; set; }
        public string BusinessDescriptionArabic { get; set; }
        public string? BusinessDescriptionEnglish { get; set; }
        public string BusinessAddressArabic { get; set; }
        public string? BusinessAddressEnglish { get; set; }
        public DayOfWeekEnum Holidays { get; set; }
        public List<string> Numbers { get; set; }
        public DateTime Opening { get; set; }
        public DateTime Closing { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string? ProfileImageUrl { get; set; }
        [NotMapped]
        public IFormFile? ProfileImage { get; set; }
        [NotMapped]
        public ICollection<IFormFile>? Images { get; set; }
        public List<string>? ImageUrls { get; set; }
        public AppUser AppUser { get; set; }
        public string UserId { get; set; }
        public int? CategoriesModelId { get; set; }
        public CategoryModel CategoryModel { get; set; }
    }
}
