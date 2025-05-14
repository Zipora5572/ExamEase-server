using Server.Core.DTOs;
using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.IServices
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<RoleDto> GetByIdAsync(int id);
        Task<RoleDto> AddRoleAsync(RoleDto role);
        Task DeleteRoleAsync(RoleDto role);
        Task<RoleDto> UpdateRoleAsync(int id, RoleDto role);
    }
}