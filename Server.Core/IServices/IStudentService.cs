using Server.Core.DTOs;
using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.IServices
{
    public interface IStudentService
    {
        Task UploadFromExcelAsync(int examId, Stream excelStream);
        Task<List<StudentDto>> GetStudentsByExamIdAsync(int examId);
        Task<List<StudentDto>> GetStudentsByTeacherIdAsync(int teacherId);
        Task<StudentDto> GetByNameAsync(string firstName, string lastName);
    }
}
