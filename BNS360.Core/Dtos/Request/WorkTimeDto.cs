using System.Text.Json.Serialization;

namespace BNS360.Core.Dtos.Request
{
    public class WorkTimeDto
    {
        public DayOfWeek Day {  get; set; }

        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly Strart { get; set; }
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly End { get; set; }

    }
}