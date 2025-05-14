using Server.Core.DTOs;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Core.Entities
{
    public class StudentExam
    {
        public int Id { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public int? StudentId { get; set; }
        public Student? Student { get; set; }
        public string ExamPath { get; set; } = "";
        public string Lang { get; set; } = "heb";



        public string NamePrefix { get; set; } = "";
        public int? FolderId { get; set; } 
        public Folder Folder { get; set; } = null;
        public int? TeacherId { get; set; }
        public User Teacher { get; set; } = null;

        public bool IsChecked { get; set; }=false;
        public int? Grade { get; set; } = 0;
        public string? Evaluation { get; set; }
        public DateTime? CheckedAt { get; set; }
    }
}
