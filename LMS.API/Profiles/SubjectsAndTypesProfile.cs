using AutoMapper;
using LMS.API.Models.DTO;
using LMS.API.Models.Entities;

namespace LMS.API.Profiles
{
    public class SubjectsAndTypesProfile : Profile
    {
        public SubjectsAndTypesProfile()
        {
            CreateMap<Subject, SubjectDto>().ReverseMap();
            CreateMap<PublicationType, PublicationTypeDto>().ReverseMap();
        }
    }
}
