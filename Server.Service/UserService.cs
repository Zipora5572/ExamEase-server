using AutoMapper;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IRepositories;
using Server.Core.IServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Service
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public UserService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _repositoryManager.Users.GetAllAsync();
            return users.ToList();
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            User user = await _repositoryManager.Users.GetByIdAsync(id);
            UserDto userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            var userDto = await _repositoryManager.Users.GetByEmailAsync(email);
            return userDto;
        }



        public async Task<UserDto> AddUserAsync(UserDto userDto)
        {
            User user = _mapper.Map<User>(userDto);
            user = await _repositoryManager.Users.AddAsync(user);
          
            await _repositoryManager.SaveAsync();
            userDto = _mapper.Map<UserDto>(user);
          
            return userDto;
        }

        public async Task DeleteUserAsync(UserDto userDto)
        {
            var existingUser=await _repositoryManager.Users.GetByIdAsync(userDto.Id);
            if (existingUser != null)
            {
                await _repositoryManager.Users.DeleteAsync(existingUser);
                await _repositoryManager.SaveAsync();
            }
            else
                throw new Exception("User Not Found");
        }

        public async Task<UserDto> UpdateUserAsync(int id, UserDto userDto)
        {
            
            User user = _mapper.Map<User>(userDto);
            user.UpdatedAt = DateTime.Now;
            user = await _repositoryManager.Users.UpdateAsync(id, user);
            await _repositoryManager.SaveAsync();
            userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<UserDto> ToggleStatusAsync(int userId)
        {
            var existingUser = await _repositoryManager.Users.GetByIdAsync(userId);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            existingUser.Status = !existingUser.Status;
            await _repositoryManager.SaveAsync();
            return _mapper.Map<UserDto>(existingUser);
        }

    }
}
