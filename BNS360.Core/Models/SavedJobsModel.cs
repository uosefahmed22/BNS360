using BNS360.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Models
{
    public class SavedJobsModel
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public JobModel JobModel { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
