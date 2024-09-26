using BNS360.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Models
{
    public class FeedbackModel
    {
        public int Id { get; set; }
        public string Feedback { get; set; }
        public int rating { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int? BusinessModelId { get; set; }
        [ForeignKey("BusinessModelId")]
        public BusinessModel BusinessModel { get; set; }
        public int? CraftsMenModelId { get; set; }
        [ForeignKey("CraftsMenModelId")]
        public CraftsMenModel CraftsMenModel { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser AppUser { get; set; }
    }
}
