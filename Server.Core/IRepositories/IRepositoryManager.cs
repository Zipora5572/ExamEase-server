using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.IRepositories
{
    public interface IRepositoryManager
    {
        IUserRepository Users { get; }
        IStudentExamRepository StudentExams { get; }
        IExamRepository Exams { get; }
        IFolderRepository Folders { get; }
        IStudentRepository Students { get; }
        IRoleRepository Roles { get; }
        IPermissionRepository Permissions { get; }
   
        IUserActivityRepository Activities { get; }
     

       Task SaveAsync();
    }
}
