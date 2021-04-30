using AutoMapper;
using LMS.Core.Entities;
using LMS.Core.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Data
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {

            CreateMap<ApplicationUser, ApplicationUserViewModel>()
            .ForMember(dest => dest.FullName,
            from => from.MapFrom(m => m.FullName))
            .ForMember(dest => dest.Course.Id,
            from => from.MapFrom(m => m.CourseId))
            .ForMember(dest => dest.Course.Title,
            from => from.MapFrom(m => m.CourseId))
            .ForMember(dest => dest.Course.Description,
            from => from.MapFrom(m => m.CourseId))
            .ForMember(dest => dest.Course.StartDate,
            from => from.MapFrom(m => m.CourseId))
            .ForMember(dest => dest.Course.EndDate,
            from => from.MapFrom(m => m.CourseId))
             .ForMember(dest => dest.Email,
            from => from.MapFrom(m => m.Email))
            ;

        }
    }
}
