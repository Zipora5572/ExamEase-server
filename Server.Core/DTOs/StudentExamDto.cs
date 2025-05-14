using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.DTOs
{
    public class StudentExamDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string? ExamPath { get; set; }
        public string Lang { get; set; } = "heb";

        public int StudentId { get; set; }
        public int? TeacherId { get; set; }
        public bool? IsChecked { get; set; } = false;
        public int? Grade { get; set; }
        public string? Evaluation { get; set; }
        public DateTime? CheckedAt { get; set; }
        public StudentDto? Student { get; set; }
        public int? FolderId { get; set; }
        public string? NamePrefix { get; set; }

    }
}
