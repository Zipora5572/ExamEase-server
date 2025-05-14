using Server.Core.DTOs;
using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server.Core.IServices
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> GetByEmailAsync(string username);
        Task<UserDto> AddUserAsync(UserDto user);
        Task DeleteUserAsync(UserDto user);
        Task<UserDto> UpdateUserAsync(int id, UserDto user);
        Task<UserDto> ToggleStatusAsync(int id);
    }
}
