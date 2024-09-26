using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Models
{
    public class CraftsModel
    {
        public int Id { get; set; }
        public string CraftsNameArabic { get; set; }
        public string? CraftsNameEnglish { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
