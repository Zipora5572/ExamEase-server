using Server.Core.DTOs;
using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.IRepositories
{
    public interface IStudentRepository:IRepository<Student>
    {
        Task AddRangeAsync(IEnumerable<Student> students);
        Task<List<Student>> GetStudentsByExamIdAsync(int examId);
        Task<List<Student>> GetStudentsByTeacherIdAsync(int teacherId);

        Task<Student> GetByNameAsync(string firstName, string lastName);
       Task<Student> GetByEmailAsync(string email);

    }
}
