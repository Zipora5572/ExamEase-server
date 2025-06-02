using Microsoft.AspNetCore.Http;
using Server.Core.DTOs;
using Server.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Core.IServices
{
    public interface IExamService
    {
        Task<List<ExamDto>> GetAllExamsAsync();
        Task<ExamDto> GetByIdAsync(int id);
        Task<List<ExamDto>> GetByUserIdAsync(int userId);
        Task<ExamDto> AddExamAsync(ExamDto exam);
        Task DeleteExamAsync(ExamDto exam);
        Task<ExamDto> UpdateExamAsync(int id, ExamDto exam, string oldName = "");
        Task<ExamDto> UploadExamAsync(ExamDto examDto, IFormFile file, int? folderId);
        Task<ExamDto> RenameExamAsync(int examId, string newName);
        string GetSignedUrl(string objectName, TimeSpan duration);
        Task<ExamDto> ToggleStarAsync(int fileId);

    }
}
