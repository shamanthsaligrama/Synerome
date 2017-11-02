using AutoMapper;

namespace SyneromeServicesAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SyneromeServices.Domain.Users,SyneromeServicesAPI.Models.Users>();
            CreateMap<SyneromeServicesAPI.Models.Users,SyneromeServices.Domain.Users>();

            CreateMap<SyneromeServices.Domain.Nutritionists, SyneromeServicesAPI.Models.Nutritionists>();
            CreateMap<SyneromeServicesAPI.Models.Nutritionists, SyneromeServices.Domain.Nutritionists>();
        }

    }
}
