using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server.Core.IServices
{
    public interface IAuthService
    {
        bool VerifyPassword(string enteredPassword, string storedPasswordHash);
        string GenerateJwtToken(string username, List<Role> roles);
        string? GetEmailFromToken(string token);
    }
}