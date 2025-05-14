using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? Password { get; set; }
        public bool Status { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public List<Role> Roles { get; set; }
    }
}
