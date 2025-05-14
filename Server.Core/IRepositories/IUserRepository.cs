using Server.Core.DTOs;
using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<UserDto> GetByEmailAsync(string email);
    }
}
