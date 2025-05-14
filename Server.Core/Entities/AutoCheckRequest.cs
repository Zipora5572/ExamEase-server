using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Entities
{
    public class AutoCheckRequest
    {
        public string StudentExamUrl { get; set; }
        public string TeacherExamUrl { get; set; }
        public string Lang { get; set; }

    }

}
