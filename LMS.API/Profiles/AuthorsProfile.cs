using AutoMapper;
using LMS.API.Helpers;
using LMS.API.Models.DTO;
using LMS.API.Models.Entities;

namespace LMS.API.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(
                        src => src.DateOfBirth.GetCurrentAge()
                    )
                );
            
            CreateMap<Author, AuthorWithPublicationsDto>()
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(
                        src => src.DateOfBirth.GetCurrentAge()
                    )
                );
        }
    }
}
