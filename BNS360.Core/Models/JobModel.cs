using BNS360.Core.Enums;
using BNS360.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Models
{
    public class JobModel
    {
        public int Id { get; set; }
        public string JobTitleArabic { get; set; }
        public string? JobTitleEnglish { get; set; }
        public string JobDescriptionArabic { get; set; }
        public string? JobDescriptionEnglish { get; set; }
        public string AddreesInArabic { get; set; }
        public string? AddreesInEnglish { get; set; }
        public List<string> Numbers { get; set; }
        public JobType Type { get; set; }
        public int WorkHours { get; set; }
        public decimal Salary { get; set; }
        public List<string> Requirements { get; set; }
        public DateTime? TimeAddedjob { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser AppUser { get; set; }
    }
}
