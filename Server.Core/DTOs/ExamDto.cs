namespace Server.Core.DTOs
{
    public class ExamDto
    {
        public int Id { get; set; } 
        public int UserId { get; set; } 
        public string Name { get; set; }

        public string NamePrefix { get; set; }
        public string Type { get; set; } = "FILE";
        public bool IsStarred { get; set; }
        public string Lang { get; set; } = "heb";

        //public string UniqueFileName { get; set; }
        public string ExamType { get; set; }
        public string ExamExtension { get; set; } 
        public long Size { get; set; } 
        public int? FolderId { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
        public bool IsDeleted { get; set; }
        public bool IsShared { get; set; }
        public string ExamPath { get; set; } 
    }
}
