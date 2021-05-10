using AutoMapper;
using LMS.API.Models.DTO;
using LMS.API.Models.Entities;

namespace LMS.API.Profiles
{
    public class PublicationsProfile : Profile
    {
        public PublicationsProfile()
        {
            CreateMap<Publication, PublicationDto>()
                .ForMember(dest => dest.TypeName,
                    opt => opt.MapFrom(
                        src => src.Type.Name
                    )
                )
                .ForMember(dest => dest.SubjectName,
                    opt => opt.MapFrom(
                        src => src.Subject.Name));
            
            CreateMap<Publication, PublicationWithAuthorsDto>()
                .ForMember(dest => dest.TypeName,
                    opt => opt.MapFrom(
                        src => src.Type.Name
                    )
                )
                .ForMember(dest => dest.SubjectName,
                    opt => opt.MapFrom(
                        src => src.Subject.Name));

            CreateMap<PublicationCreationDto, Publication>();
        }
    }
}
