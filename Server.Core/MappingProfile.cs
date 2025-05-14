using AutoMapper;
using Server.Core.DTOs;
using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Folder, FolderDto>().ReverseMap();
            CreateMap<Exam, ExamDto>().ReverseMap();
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
          
            CreateMap<StudentExam, StudentExamDto>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();
        }
    }
}
