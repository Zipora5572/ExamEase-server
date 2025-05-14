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
    public class RoleService : IRoleService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public RoleService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            var roles = await _repositoryManager.Roles.GetAllAsync();
            return roles.ToList();
        }

        public async Task<RoleDto> GetByIdAsync(int id)
        {
            Role role = await _repositoryManager.Roles.GetByIdAsync(id);
            RoleDto roleDto = _mapper.Map<RoleDto>(role);
            return roleDto;
        }

        public async Task<RoleDto> AddRoleAsync(RoleDto roleDto)
        {
            Role role = _mapper.Map<Role>(roleDto);
            role = await _repositoryManager.Roles.AddAsync(role);
            await _repositoryManager.SaveAsync();
            roleDto = _mapper.Map<RoleDto>(role);
            return roleDto;
        }

        public async Task DeleteRoleAsync(RoleDto roleDto)
        {
            Role role = _mapper.Map<Role>(roleDto);
            await _repositoryManager.Roles.DeleteAsync(role);
            await _repositoryManager.SaveAsync();
        }

        public async Task<RoleDto> UpdateRoleAsync(int id, RoleDto roleDto)
        {
            Role role = _mapper.Map<Role>(roleDto);
            role = await _repositoryManager.Roles.UpdateAsync(id, role);
            await _repositoryManager.SaveAsync();
            roleDto = _mapper.Map<RoleDto>(role);
            return roleDto;
        }



    }
}
