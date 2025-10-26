using AutoMapper;
using SecSchoolApi.Model;

namespace SecSchoolApi.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUser>().ReverseMap();

            CreateMap<StudentModel, StudentModel>().ReverseMap();

            CreateMap<ParentModel, ParentModel>().ReverseMap();

            CreateMap<Teacher, Teacher>().ReverseMap();

            CreateMap<FeePayment, FeePayment>().ReverseMap();

            CreateMap<Result, Result>().ReverseMap();

            CreateMap<AttendanceModel, AttendanceModel>().ReverseMap();

            CreateMap<Assignment, Assignment>().ReverseMap();

            CreateMap<AnnouncementModel, AnnouncementModel>().ReverseMap();

            CreateMap<Message, Message>().ReverseMap();

            CreateMap<Notification, Notification>().ReverseMap();

            CreateMap<School, School>().ReverseMap();
            CreateMap<Branding, Branding>().ReverseMap();

            CreateMap<Class, Class>().ReverseMap();
        }
    }
}
