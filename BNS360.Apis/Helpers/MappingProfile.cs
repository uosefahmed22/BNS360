using AutoMapper;
using BNS360.Core.Dto;
using BNS360.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BNS360.Apis.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BusinessModelDto, BusinessModel>().ReverseMap();
            CreateMap<CategoryModelDto, CategoryModel>().ReverseMap();
            CreateMap<CraftsModelDto, CraftsModel>().ReverseMap();
            CreateMap<CraftsMenModelDto, CraftsMenModel>().ReverseMap();
        }
    }
}
