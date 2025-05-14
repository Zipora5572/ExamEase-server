using Microsoft.EntityFrameworkCore;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        readonly IDataContext _context;

        public StudentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Student> students)
        {
            await _context.Students.AddRangeAsync(students);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Student>> GetStudentsByExamIdAsync(int examId)
        {
            var students = await _context.StudentExams
                .Where(se => se.ExamId == examId)
                .Include(se => se.Student)
                .Select(se => se.Student)
                .Distinct()
                .ToListAsync();

            return students;
        }
        public async Task<List<Student>> GetStudentsByTeacherIdAsync(int teacherId)
        {
            var students = await _context.StudentExams
                .Where(se => se.TeacherId == teacherId)
                .Include(se => se.Student)
                .Select(se => se.Student)
                .Distinct()
                .ToListAsync();

            return students;
        }



        public async Task<Student> GetByNameAsync(string firstName, string lastName)
        {
            if (_context == null)
                throw new Exception("_context is null");
            if (_context.Students == null)
                throw new Exception("_context.Students is null");
          
            return await _context.Students
             .FirstOrDefaultAsync(s => s.FirstName == firstName && s.LastName == lastName);
        }
        public async Task<Student> GetByEmailAsync(string email)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Email == email);
        }
    }


}

