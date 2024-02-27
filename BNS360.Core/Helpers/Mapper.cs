using BNS360.Core.Dtos.Response.AppBusniss;
using BNS360.Core.Entities;
using BNS360.Core.Services.AppBusniss;
using BNS360.Core.Services.Shared;
namespace BNS360.Core.Helpers;

public static class Mapper
{

    public static BusnissReponse MapToBusnissResponse(this Business? busniss,
         IReviewService ReviewService,
         IFileService FileService)
    {
        ArgumentNullException.ThrowIfNull(busniss, nameof(busniss));
        
        return new BusnissReponse
        {
            Id = busniss.Id,
            NameAR = busniss.NameAR,
            DescriptionAR = busniss.AboutAR,
            NameENG = busniss.NameENG,
            DescriptionENG = busniss.AboutENG,
            ReviewSummary = ReviewService.GetReviewSummary(busniss.Reviews),
            ProfilePictureUrl = FileService.GetAbsoluteFilePath(busniss.ProfilePictureUrl)
        };
    }
}
