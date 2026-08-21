using BNS360.Core.Dto;
using BNS360.Core.Models;

namespace BNS360.Repository.Mapping;

internal static class ModelMappingExtensions
{
    public static BusinessModel ToEntity(this BusinessModelDto source)
    {
        var target = new BusinessModel { UserId = source.userId! };
        source.ApplyTo(target);
        return target;
    }

    public static void ApplyTo(this BusinessModelDto source, BusinessModel target)
    {
        target.BusinessNameArabic = source.BusinessNameArabic;
        target.BusinessNameEnglish = source.BusinessNameEnglish;
        target.BusinessDescriptionArabic = source.BusinessDescriptionArabic;
        target.BusinessDescriptionEnglish = source.BusinessDescriptionEnglish;
        target.BusinessAddressArabic = source.BusinessAddressArabic;
        target.BusinessAddressEnglish = source.BusinessAddressEnglish;
        target.Holidays = source.Holidays;
        target.Numbers = source.Numbers;
        target.Opening = source.Opening;
        target.Closing = source.Closing;
        target.Longitude = source.Longitude;
        target.Latitude = source.Latitude;
        target.CategoriesModelId = source.CategoriesModelId;

        if (source.ProfileImageUrl is not null)
        {
            target.ProfileImageUrl = source.ProfileImageUrl;
        }

        if (source.ImageUrls is not null)
        {
            target.ImageUrls = source.ImageUrls;
        }
    }

    public static CategoryModel ToEntity(this CategoryModelDto source) => new()
    {
        CategoryNameArabic = source.CategoryNameArabic,
        CategoryNameEnglish = source.CategoryNameEnglish,
        ImageUrl = source.ImageUrl
    };

    public static CraftsModel ToEntity(this CraftsModelDto source) => new()
    {
        CraftsNameArabic = source.CraftsNameArabic,
        CraftsNameEnglish = source.CraftsNameEnglish
    };

    public static CraftsMenModel ToEntity(this CraftsMenModelDto source)
    {
        var target = new CraftsMenModel { UserId = source.UserId! };
        source.ApplyTo(target);
        return target;
    }

    public static void ApplyTo(this CraftsMenModelDto source, CraftsMenModel target)
    {
        target.CraftsMenNameArabic = source.CraftsMenNameArabic;
        target.CraftsMenNameEnglish = source.CraftsMenNameEnglish;
        target.CraftsMenDescriptionArabic = source.CraftsMenDescriptionArabic;
        target.CraftsMenDescriptionEnglish = source.CraftsMenDescriptionEnglish;
        target.CraftsMenAddressArabic = source.CraftsMenAddressArabic;
        target.CraftsMenAddressEnglish = source.CraftsMenAddressEnglish;
        target.Holidays = source.Holidays;
        target.Numbers = source.Numbers;
        target.Opening = source.Opening;
        target.Closing = source.Closing;
        target.CraftsModelId = source.CraftsModelId;

        if (source.ProfileImageUrl is not null)
        {
            target.ProfileImageUrl = source.ProfileImageUrl;
        }

        if (source.ImageUrls is not null)
        {
            target.ImageUrls = source.ImageUrls;
        }
    }

    public static JobModel ToEntity(this JobModelDto source)
    {
        var target = new JobModel { UserId = source.UserId! };
        source.ApplyTo(target);
        return target;
    }

    public static void ApplyTo(this JobModelDto source, JobModel target)
    {
        target.JobTitleArabic = source.JobTitleArabic;
        target.JobTitleEnglish = source.JobTitleEnglish;
        target.JobDescriptionArabic = source.JobDescriptionArabic;
        target.JobDescriptionEnglish = source.JobDescriptionEnglish;
        target.AddreesInArabic = source.AddreesInArabic;
        target.AddreesInEnglish = source.AddreesInEnglish;
        target.Numbers = source.Numbers;
        target.Type = source.Type;
        target.WorkHours = source.WorkHours;
        target.Salary = source.Salary;
        target.Requirements = source.Requirements;
    }

    public static PropertyModel ToEntity(this PropertyModelDto source)
    {
        var target = new PropertyModel { UserId = source.userId! };
        source.ApplyTo(target);
        return target;
    }

    public static void ApplyTo(this PropertyModelDto source, PropertyModel target)
    {
        target.ArabicDescription = source.ArabicDescription;
        target.EnglishDescription = source.EnglishDescription;
        target.ArabicAddress = source.ArabicAddress;
        target.EnglishAddress = source.EnglishAddress;
        target.Numbers = source.Numbers;
        target.Type = source.Type;
        target.Area = source.Area;
        target.Price = source.Price;
        target.Longitude = source.Longitude;
        target.Latitude = source.Latitude;

        if (source.ImageUrls is not null)
        {
            target.ImageUrls = source.ImageUrls;
        }
    }
}
