using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.IRepositories
{
    public interface IStudentExamRepository:IRepository<StudentExam>
    {
        Task<List<StudentExam>> GetByExamIdAsync(int examId);
        Task<StudentExam> GetByStudentIdAsync(int studentId);
        Task<bool> ExistsAsync(int studentId, int examId);
        Task AddRangeAsync(IEnumerable<StudentExam> studentsExams);


    }
}
