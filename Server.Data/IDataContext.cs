using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Server.Core.Entities;

namespace Server.Data
{
    public interface IDataContext
    {
        public DbSet<Exam> Exams { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
      
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        Task<int> SaveChangesAsync(); 
        public EntityEntry Entry(object entity);
    }
}
