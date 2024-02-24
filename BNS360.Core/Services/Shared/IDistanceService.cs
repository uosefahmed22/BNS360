using BNS360.Core.Dtos.Request;
using BNS360.Core.Entities;

namespace BNS360.Core.Services.Shared;

public interface IDistanceService
{
    double FindDistance(CurrentLocation currentLocation, Location location); 
}
