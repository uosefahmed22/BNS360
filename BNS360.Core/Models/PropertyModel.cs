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
    public class PropertyModel
    {
        public int Id { get; set; }
        public string ArabicDescription { get; set; }
        public string? EnglishDescription { get; set; }
        public string ArabicAddress { get; set; }
        public string? EnglishAddress { get; set; }
        public List<string> Numbers { get; set; }
        public PropertyType Type { get; set; }
        public int Area { get; set; }
        public decimal Price { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public DateTime TimeAddedProperty { get; set; } = DateTime.Now;
        [NotMapped]
        public ICollection<IFormFile>? Images { get; set; }
        public List<string>? ImageUrls { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser AppUser { get; set; }

    }
}
