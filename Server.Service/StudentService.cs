using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Server.Core.Entities;
using Server.Core.IServices;
using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Server.Core.IRepositories;
using ClosedXML.Excel;
using Server.Core.DTOs;
using AutoMapper; 

namespace Server.Service
{
    public class StudentService : IStudentService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IStudentExamService _studentExamService;
        private readonly IExamService _examService;
        private readonly IMapper _mapper;
        public StudentService(IRepositoryManager repositoryManager,IMapper mapper, IStudentExamService studentExamService, IExamService examService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _studentExamService = studentExamService;
            _examService = examService;
        }
        public async Task<List<StudentDto>> GetStudentsByExamIdAsync(int examId)
        {
            var students = await _repositoryManager.Students.GetStudentsByExamIdAsync(examId);
            return _mapper.Map<List<StudentDto>>(students);
        }
        public async Task<List<StudentDto>> GetStudentsByTeacherIdAsync(int teacherId)
        {
            var students = await _repositoryManager.Students.GetStudentsByTeacherIdAsync(teacherId);
            return _mapper.Map<List<StudentDto>>(students);
        }


        public async Task<StudentDto> GetByNameAsync(string firstName, string lastName)
        {
            var student = await _repositoryManager.Students.GetByNameAsync(firstName, lastName);
         
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> GetByEmailAsync(string email)
        {
            var student = await _repositoryManager.Students.GetByEmailAsync(email);
            return _mapper.Map<StudentDto>(student);
        }

        public async Task UploadFromExcelAsync(int examId, Stream excelStream)
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                throw new Exception("Excel file is empty.");

            var students = new List<StudentDto>();
            var studentExams = new List<StudentExam>();
            int row = 2;

            while (!worksheet.Cell(row, 1).IsEmpty())
            {
                var firstName = worksheet.Cell(row, 1).GetValue<string>();
                var lastName = worksheet.Cell(row, 2).GetValue<string>();
                var email = worksheet.Cell(row, 3).GetValue<string>();

                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                {
                    row++;
                    continue;
                }

                var student = await GetByEmailAsync(email);

                if (student == null)
                {
                    student = new StudentDto
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                    };

                    students.Add(student);
                }
                else
                {
                    // אם התלמיד קיים, השתמש ב-ID שלו
                }

                row++;
               

            }


            if (students.Any())
            {
                await _repositoryManager.Students.AddRangeAsync(_mapper.Map<List<Student>>(students));
                await _repositoryManager.SaveAsync(); 
            }

         
            foreach (var student in students)
            {

                var savedStudent = await _repositoryManager.Students.GetByEmailAsync(student.Email); 

                var exists = await _repositoryManager.StudentExams.ExistsAsync(savedStudent.Id, examId);
                if (!exists)
                {
                    var exam=await _examService.GetByIdAsync(examId);
                 
                    var studentExam = new StudentExam
                    {
                        StudentId = savedStudent.Id,
                        ExamId = examId,
                        TeacherId=exam.UserId
                    };
                   
                    studentExams.Add(studentExam);
                }
            }
            if (studentExams.Any())
            {
                await _repositoryManager.StudentExams.AddRangeAsync(studentExams);
            }
            await _repositoryManager.SaveAsync();
        }



    }
}
