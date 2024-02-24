using BNS360.Core.Dtos.Request;
using BNS360.Core.Entities;
using BNS360.Core.Services.Shared;

namespace BNS360.Reposatory.Repositories.Shared;

public class DistanceService : IDistanceService
{
    private const double _earthRadius = 6371;

    public double FindDistance(CurrentLocation currentLocation, Location location)
    {
        var dLat = Math.PI / 180 * (double)(currentLocation.Latitude - location.Latitude);
        var dLon = Math.PI / 180 * (double)(currentLocation.Longitude - location.Longitude);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Math.PI / 180 * (double)location.Latitude) * Math.Cos(Math.PI / 180 * (double)currentLocation.Latitude) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distance = _earthRadius * c; // Distance in kilometers

        return distance;

    }
}
