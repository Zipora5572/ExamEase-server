using Microsoft.AspNetCore.Mvc;
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
    public class StudentExamRepository : Repository<StudentExam>, IStudentExamRepository
    {
        readonly DataContext _context;
        private readonly DbSet<StudentExam> _dbSet;

        public StudentExamRepository(DataContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<StudentExam>();
        }
        public async Task AddRangeAsync(IEnumerable<StudentExam> studentsExams)
        {
            await _context.StudentExams.AddRangeAsync(studentsExams);
            await _context.SaveChangesAsync();
        }
        public async Task<List<StudentExam>> GetByExamIdAsync(int examId)
        {

            return await _context.StudentExams
                .Where(se => se.ExamId == examId)
                .Include(se => se.Student)
                .ToListAsync();
        }

        public async Task<StudentExam> GetByStudentIdAsync(int studentId)
        {

            return await _context.StudentExams
                   .Where(se => se.StudentId == studentId)
                   .Include(se => se.Student)
                   .FirstOrDefaultAsync();
        }

        public async Task<StudentExam?> UpdateAsync(int id, StudentExam entity)
        {
            var existingEntity = await _dbSet.Include(se => se.Student).FirstOrDefaultAsync(se => se.Id == id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;

                var properties = typeof(StudentExam).GetProperties();
                foreach (var property in properties)
                {
                    if (property.CanWrite && property.Name != "Id")
                    {
                        var newValue = property.GetValue(entity);
                        property.SetValue(existingEntity, newValue);
                    }
                }
                _context.Entry(existingEntity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return existingEntity;
            }
            return null;
        }
        public async Task<bool> ExistsAsync(int studentId, int examId)
        {
            return await _context.StudentExams
                .AnyAsync(se => se.StudentId == studentId && se.ExamId == examId);
        }



    }
}
