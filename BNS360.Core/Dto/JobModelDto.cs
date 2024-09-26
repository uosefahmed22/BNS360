using BNS360.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Dto
{
    public class JobModelDto
    {
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
        public string? UserId { get; set; }
    }
}
