using AutoMapper;
using Robotic.Forklift.Application.Dtos;

namespace Robotic.Forklift.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.Forklift, ForkliftDto>();
        }
    }
}
