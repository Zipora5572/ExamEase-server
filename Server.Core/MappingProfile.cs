using AutoMapper;
using Server.Core.DTOs;
using Server.Core.Entities;
using System.Linq;

namespace Server.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Folder, FolderDto>().ReverseMap();
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<StudentExam, StudentExamDto>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();

            CreateMap<Exam, ExamDto>()
                .AfterMap((src, dest) =>
                {
                    dest.Submissions = src.StudentExams?.Count ?? 0;

                    var gradedExams = src.StudentExams?.Where(se => se.Grade.HasValue && se.Grade.Value > 0);
                    dest.AverageGrade = (gradedExams != null && gradedExams.Any())
                        ? gradedExams.Average(se => se.Grade.Value)
                        : (double?)null;

                    if (dest.Submissions == 0)
                        dest.Status = ExamDto.StatusEnum.Pending;
                    else if (dest.Submissions > 0 && dest.AverageGrade == null)
                        dest.Status = ExamDto.StatusEnum.InProgress;
                    else
                        dest.Status = ExamDto.StatusEnum.Completed;
                })
                .ReverseMap();
        }
    }
}
