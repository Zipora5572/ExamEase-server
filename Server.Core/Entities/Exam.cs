using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Entities
{
    public class Exam
    {
        public int Id { get; set; }

        public int UserId { get; set; }
       

        public User User { get; set; }


        public string Name { get; set; }
       
        public string NamePrefix { get; set; }
        public string Lang { get; set; } = "heb";
        public string Type { get; set; } = "FILE";

        
        //public string UniqueFileName { get; set; }


        public string ExamType { get; set; }
        public string ExamExtension { get; set; }

        public long Size { get; set; }

        
        public int ?FolderId { get; set; } 
        public Folder? Folder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
        public bool IsShared { get; set; }
        public bool IsStarred { get; set; } = false;


        public string ExamPath { get; set; }

       
    }
}
