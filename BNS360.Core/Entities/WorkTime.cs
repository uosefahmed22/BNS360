using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Entities
{
    public class WorkTime : BaseEntity
    {
        public required DayOfWeek Day { get; set; }
        public required TimeOnly Start { get; set; }
        public required TimeOnly End { get; set; }
        [Required]
        [ForeignKey("BusnissId")]
        public Guid BusnissId { get; set; }
    }
}
