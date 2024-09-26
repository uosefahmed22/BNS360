using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string CategoryNameArabic { get; set; }
        public string? CategoryNameEnglish { get; set; }
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        public List<BusinessModel> BusinessModels { get; set; }
    }
}
