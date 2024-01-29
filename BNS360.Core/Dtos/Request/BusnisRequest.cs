
using BNS360.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace BNS360.Core.Dtos.Request
{
    public class BusnisRequest
    {
        public required string Name { get; set; }
        public required string About { get; set; }
        public IFormFile? Picture { get; set; }
        public Guid CategoryId { get; set; }
        public required LocationDto Location { get; set; }
        public List<WorkTimeDto> WorkTime { get; set; } = new();
        public required ContactDto ContactInfo { get; set; }
    }
}
