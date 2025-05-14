using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Entities
{
    public class UserActivity
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string PagePath { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

}
