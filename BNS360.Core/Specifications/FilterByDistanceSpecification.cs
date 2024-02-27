using BNS360.Core.Dtos.Request;
using BNS360.Core.Entities;
using BNS360.Core.Services.Shared;

namespace BNS360.Core.Specifications
{
    public class FilterByDistanceSpecification : BaseSpecification<Business>
    {
        private readonly IDistanceService _distanceService;
        public FilterByDistanceSpecification(CurrentLocation currentLocation, int pageNumber, int size, IDistanceService distanceService)
        {
            PageIndex = pageNumber - 1;
            PageSize = size;
            _distanceService = distanceService;
            OrderByExpression = b => _distanceService.FindDistance(currentLocation, b.Location!);
        }

    }

}