
using AutoMapper;
using Google.Apis.Storage.v1;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IRepositories;
using Server.Core.IServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Service
{
    public class StudentExamService : IStudentExamService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;

        public StudentExamService(IRepositoryManager repositoryManager, IMapper mapper, IStorageService storageService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _storageService = storageService;
        }

        public async Task<List<StudentExamDto>> GetAllStudentExamsAsync()
        {
            var studentExams = await _repositoryManager.StudentExams.GetAllAsync();
            return studentExams.Select(exam => _mapper.Map<StudentExamDto>(exam)).ToList();
        }

        public async Task<StudentExamDto> GetByIdAsync(int id)
        {
            StudentExam studentExam = await _repositoryManager.StudentExams.GetByIdAsync(id);
            return _mapper.Map<StudentExamDto>(studentExam);
        }

        public async Task<StudentExamDto> GetByStudentIdAsync(int studentId)
        {
            var studentExam = await _repositoryManager.StudentExams.GetByStudentIdAsync(studentId);
            return _mapper.Map<StudentExamDto>(studentExam);
        }
        public async Task<List<StudentExamDto>> GetStudentExamsByExamIdAsync(int examId)
        {
            var studentExams = await _repositoryManager.StudentExams.GetByExamIdAsync(examId);
            return studentExams.Select(exam => _mapper.Map<StudentExamDto>(exam)).ToList();
        }

        public async Task<StudentExamDto> AddStudentExamAsync(StudentExamDto studentExamDto)
        {
            
            StudentExam studentExam = _mapper.Map<StudentExam>(studentExamDto);
            studentExam = await _repositoryManager.StudentExams.AddAsync(studentExam);
            await _repositoryManager.SaveAsync();
            return _mapper.Map<StudentExamDto>(studentExam);
        }

        public async Task<StudentExamDto> UpdateStudentExamAsync(int id, StudentExamDto studentExamDto)
        {
            StudentExam studentExam = _mapper.Map<StudentExam>(studentExamDto);
            studentExam = await _repositoryManager.StudentExams.UpdateAsync(id, studentExam);
            await _repositoryManager.SaveAsync();
            return _mapper.Map<StudentExamDto>(studentExam);
        }

        public async Task DeleteStudentExamAsync(StudentExamDto studentExamDto)
        {
            StudentExam studentExam = _mapper.Map<StudentExam>(studentExamDto);
            await _repositoryManager.StudentExams.DeleteAsync(studentExam);
            await _repositoryManager.SaveAsync();
        }
        public async Task ReplaceCorrectedImageAsync(int studentExamId, Stream correctedImageStream)
        {
            var studentExam = await _repositoryManager.StudentExams.GetByIdAsync(studentExamId);
            if (studentExam == null || string.IsNullOrEmpty(studentExam.ExamPath))
            {
                throw new Exception("Student exam or file URL not found.");
            }

            await _storageService.ReplaceFileAsync(studentExam.NamePrefix, correctedImageStream);
        }

    }
}
