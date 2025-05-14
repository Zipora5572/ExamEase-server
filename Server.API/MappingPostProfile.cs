using AutoMapper;
using Server.API.PostModel;
using Server.Core.DTOs;
using Server.Core.Entities;

namespace Server.API
{
    public class MappingPostProfile:Profile
    {
        public MappingPostProfile()
        {
            CreateMap<UserPostModel, UserDto>();
            CreateMap<ExamPostModel, ExamDto>();
            CreateMap<FolderPostModel, FolderDto>();
            CreateMap<RolePostModel, RoleDto>();
            CreateMap<PermissionPostModel, PermissionDto>();
            CreateMap<StudentExamPostModel, StudentExamDto>();
            CreateMap<StudentPostModel, StudentDto>();

        }
    }
}
