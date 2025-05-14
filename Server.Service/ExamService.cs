using AutoMapper;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IRepositories;
using Server.Core.IServices;


namespace Server.Service
{
    public class ExamService : IExamService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
     

        public ExamService(IRepositoryManager repositoryManager, IMapper mapper, IStorageService storageService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _storageService = storageService;      
        }

        public async Task<List<Exam>> GetAllExamsAsync()
        {

            var exams = await _repositoryManager.Exams.GetAllAsync();
            return exams.ToList();
        }

        public async Task<ExamDto> GetByIdAsync(int id)
        {
            Exam exam = await _repositoryManager.Exams.GetByIdAsync(id);
            ExamDto examDto = _mapper.Map<ExamDto>(exam);
            return examDto;
        }
        public async Task<ExamDto> UploadExamAsync(
            ExamDto examDto,
            IFormFile file,
            int? folderId)
        {
          
            string objectName;
            if (folderId.HasValue)
            {
                var folder = await _repositoryManager.Folders.GetByIdAsync(folderId.Value);

                string Name = folder?.Name ?? "";
                objectName = $"{Name}/{file.FileName}";
            }
            else
            {
                objectName = file.FileName;
            } 
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var tempFilePath = Path.Combine(Path.GetTempPath(), uniqueFileName);
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }       
         
            await _repositoryManager.SaveAsync();

          
            examDto.NamePrefix = objectName;
            examDto.Name = Path.GetFileNameWithoutExtension(file.FileName);
            examDto.ExamExtension = Path.GetExtension(file.FileName);
            examDto.ExamPath = $"https://storage.cloud.google.com/exams-bucket/{objectName}";
            examDto.Size = file.Length;
            examDto.ExamType = file.ContentType;
            examDto.CreatedAt = DateTime.Now;
            examDto.UpdatedAt = DateTime.Now;
            examDto.IsDeleted = false;          
            var exam = _mapper.Map<Exam>(examDto);
            var addedExam = await _repositoryManager.Exams.AddAsync(exam);
            await _repositoryManager.SaveAsync();       
            await _storageService.UploadFileAsync(tempFilePath, objectName);       
            File.Delete(tempFilePath);
            return _mapper.Map<ExamDto>(addedExam);
        }

        public async Task<ExamDto> AddExamAsync(ExamDto examDto)
        {
            Exam exam = _mapper.Map<Exam>(examDto);
            exam = await _repositoryManager.Exams.AddAsync(exam);
            await _repositoryManager.SaveAsync();
            examDto = _mapper.Map<ExamDto>(exam);
            return examDto;
        }

        public async Task DeleteExamAsync(ExamDto examDto)
        {
            var existingExam = await _repositoryManager.Exams.GetByIdAsync(examDto.Id);
            if (existingExam != null)
            {
                await _repositoryManager.Exams.DeleteAsync(existingExam);
                await _storageService.DeleteFileAsync(examDto.NamePrefix);
                await _repositoryManager.SaveAsync();
            }
            else
            {
                throw new InvalidOperationException("Exam not found.");
            }
        }

        public async Task<ExamDto> UpdateExamAsync(int id, ExamDto examDto, string oldName = "")
        {
            Exam exam = _mapper.Map<Exam>(examDto);

            exam = await _repositoryManager.Exams.UpdateAsync(id, exam);
           

            if (!string.IsNullOrEmpty(oldName))
            {
                string folderPath = Path.GetDirectoryName(oldName);
                string newFileName = examDto.Name + examDto.ExamExtension;
                string newFilePath = Path.Combine(folderPath, newFileName);
                await _storageService.RenameFileAsync(oldName, newFilePath);
            }
            await _repositoryManager.SaveAsync();
            examDto = _mapper.Map<ExamDto>(exam);
            return examDto;
        }
        public async Task<ExamDto> RenameExamAsync(int examId, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException("New name cannot be null or empty.");

            var examDto = await GetByIdAsync(examId);
            if (examDto == null)
                return null;

            string oldName = examDto.NamePrefix;
            var parts = oldName.Split("\\").ToList();
            if (parts.Any())
            {
                parts[parts.Count - 1] = newName;
                examDto.NamePrefix = string.Join("\\", parts) + examDto.ExamExtension;
            }

            examDto.Name = newName;
            return await UpdateExamAsync(examId, examDto, oldName);
        }
        public string GetSignedUrl(string objectName, TimeSpan duration)
        {
            var credentialPath = @"C:\Users\user1\Desktop\ציפי לימודים שנה ב\Fullstack Project\Web API .NET\Server.service\exams-management-service.json";
            var urlSigner = UrlSigner.FromServiceAccountPath(credentialPath);

            return urlSigner.Sign(
              "exams-bucket",
                objectName,
                duration,
                HttpMethod.Get
            );
        }
       

        public async Task<ExamDto> ToggleStarAsync(int fileId)
        {
            var file = await _repositoryManager.Exams.GetByIdAsync(fileId);
            if (file == null) return null;

            file.IsStarred = !file.IsStarred;
            await _repositoryManager.SaveAsync();

            return _mapper.Map<ExamDto>(file);
        }



    }
}
