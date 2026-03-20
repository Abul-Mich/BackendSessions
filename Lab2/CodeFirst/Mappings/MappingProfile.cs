using AutoMapper;
using DbApi.CodeFirst;
using DbApi.DTOs;
using DbApi.ViewModels;

namespace DbApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Student → StudentViewModel (with nested enrollments)
            CreateMap<CfStudent, StudentViewModel>()
                .ForMember(d => d.Enrollments, o => o.MapFrom(s => s.Enrollments));

            // Enrollment inside Student (no student reference to avoid cycle)
            CreateMap<CfEnrollment, StudentEnrollmentViewModel>()
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Title));

            // Course → CourseViewModel
            CreateMap<CfCourse, CourseViewModel>()
                .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher.Name));

            // Enrollment → EnrollmentViewModel (flat, no cycles)
            CreateMap<CfEnrollment, EnrollmentViewModel>()
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Name))
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Title));

            // DTOs → Entities
            CreateMap<CreateStudentDto, CfStudent>();
            CreateMap<CreateTeacherDto, CfTeacher>();
            CreateMap<CreateCourseDto, CfCourse>();
        }
    }
}
