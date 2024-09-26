using BNS360.Core.Enums;
using BNS360.Core.Models.Auth;
using BNS360.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Dto
{
    public class CraftsMenModelDto
    {
        public string CraftsMenNameArabic { get; set; }
        public string? CraftsMenNameEnglish { get; set; }
        public string CraftsMenDescriptionArabic { get; set; }
        public string? CraftsMenDescriptionEnglish { get; set; }
        public string CraftsMenAddressArabic { get; set; }
        public string? CraftsMenAddressEnglish { get; set; }
        public DayOfWeekEnum Holidays { get; set; }
        public List<string> Numbers { get; set; }
        public DateTime Opening { get; set; }
        public DateTime Closing { get; set; }
        public string? ProfileImageUrl { get; set; }
        [NotMapped]
        public IFormFile ProfileImage { get; set; }
        [NotMapped]
        public ICollection<IFormFile>? Images { get; set; }
        public List<string>? ImageUrls { get; set; }
        public string? UserId { get; set; }
        public int? CraftsModelId { get; set; }
    }
}
