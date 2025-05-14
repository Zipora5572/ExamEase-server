using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Entities
{
    public class AutoCheckResponse
    {
        public string? Grade { get; set; }
        public string Evaluation { get; set; } = string.Empty;
    }

}
