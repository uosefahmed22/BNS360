using BNS360.Core.Helpers;

namespace BNS360.Core.Entities;

public class ServiceProvider : MainEntity
{
    public Guid ServiceId {  get; set; } 
    public Service? Service { get; set; }
}