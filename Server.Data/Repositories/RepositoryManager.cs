using Server.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server.Data.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly IDataContext _context;

        public IUserRepository Users { get; }
        public IExamRepository Exams { get; }
        public IStudentExamRepository StudentExams { get; }
        public IFolderRepository Folders { get; }
      
        public IPermissionRepository Permissions { get; }
        public IStudentRepository Students { get; }
        public IRoleRepository Roles { get; }
       
        public IUserActivityRepository Activities { get; }

       

        public RepositoryManager(
            IDataContext context,
            IUserRepository userRepository,
            IExamRepository examRepository,
            IStudentExamRepository studentExamRepository,
            IFolderRepository folderRepository,
            IStudentRepository studentRepository,
            IPermissionRepository permissionRepository,
            IRoleRepository roleRepository,
            IUserActivityRepository activities)
        {
            _context = context;
            Users = userRepository;
            Exams = examRepository;
            Folders = folderRepository;
            StudentExams = studentExamRepository;
            Permissions = permissionRepository;
            Students = studentRepository;
            Roles = roleRepository;
            Activities=activities;
        }

        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
