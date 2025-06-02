using Microsoft.EntityFrameworkCore;
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
    public class ExamRepository : Repository<Exam>, IExamRepository
    {
        readonly IDataContext _context;
        public ExamRepository(DataContext context) : base(context)
        {
            _context = context;
        }


        public IQueryable<Exam> GetAllExams() => _context.Exams;
        public async Task<Exam> GetByIdAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.StudentExams)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Exam>> GetByUserIdAsync(int userId)
        {
            return await _context.Exams
                .Where(e => e.UserId == userId)
                .Include(e => e.StudentExams)
                .ToListAsync();
        }

    }
}
