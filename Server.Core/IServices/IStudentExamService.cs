using Server.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Core.IServices
{
    public interface IStudentExamService
    {
        Task<List<StudentExamDto>> GetAllStudentExamsAsync();
        Task<StudentExamDto> GetByIdAsync(int id);
        Task<StudentExamDto> AddStudentExamAsync(StudentExamDto studentExamDto);
        Task<StudentExamDto> UpdateStudentExamAsync(int id, StudentExamDto studentExamDto);
        Task DeleteStudentExamAsync(StudentExamDto studentExamDto);
        Task<List<StudentExamDto>> GetStudentExamsByExamIdAsync(int examId);
        Task ReplaceCorrectedImageAsync(int studentExamId, Stream correctedImageStream);
        Task<StudentExamDto> GetByStudentIdAsync(int studentId);
    }
}
