using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Server.Core.Entities;


namespace Server.Data
{
    public class DataContext : DbContext, IDataContext
    {
        public DbSet<Exam> Exams { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
      
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students{ get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual EntityEntry Entry(object entity)
        {
            return base.Entry(entity);
        }
        public async Task<int> SaveChangesAsync() 
        {
            return await base.SaveChangesAsync();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

           

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Folder)
                .WithMany()
                .HasForeignKey(e => e.FolderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StudentExam>()
                .HasOne(se => se.Exam)
                .WithMany()
                .HasForeignKey(se => se.ExamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentExam>()
                .HasOne(se => se.Student)
                .WithMany()
                .HasForeignKey(se => se.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentExam>()
                .HasOne(se => se.Teacher)
                .WithMany()
                .HasForeignKey(se => se.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Folder>()
                .HasQueryFilter(f => !f.IsDeleted);

            modelBuilder.Entity<StudentExam>()
    .HasOne(se => se.Folder)
    .WithMany()
    .HasForeignKey(se => se.FolderId)
    .OnDelete(DeleteBehavior.Restrict); // או DeleteBehavior.NoAction

        }



    }
}
