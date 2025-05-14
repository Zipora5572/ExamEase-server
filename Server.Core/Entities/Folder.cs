using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Entities
{
    public class Folder
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsStarred { get; set; }=false;

        public string Name { get; set; }
        public string NamePrefix { get; set; } = "";
        public string Type { get; set; } = "FOLDER";

        [ForeignKey(nameof(Folder))]
        public int? ParentFolderId { get; set; }
        public Folder? ParentFolder { get; set; }
        public bool OfTeacherExams { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

    }
}
