using BNS360.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.Models
{
    public class FavoriteModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser AppUser { get; set; }
        public int? businessId { get; set; }
        [ForeignKey("businessId")]
        public BusinessModel BusinessModel { get; set; }
        public int? CraftsMenId { get; set; }
        [ForeignKey("CraftsMenId")]
        public CraftsMenModel CraftsMenModel { get; set; }
    }
}
