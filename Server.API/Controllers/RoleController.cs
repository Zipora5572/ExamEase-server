using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.API.PostModel; 
using Server.Core.DTOs; 
using Server.Core.Entities;
using Server.Core.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        // GET: api/<RoleController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> Get()
        {
            List<Role> roles = await _roleService.GetAllRolesAsync();
            if (roles == null)
            {
                return NotFound();
            }
            var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            return Ok(roleDtos);
        }

        // GET api/<RoleController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> Get(int id)
        {
            RoleDto role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var roleDto = _mapper.Map<RoleDto>(role);
            return Ok(roleDto);
        }

        // POST api/<RoleController>
        [HttpPost]
        public async Task<ActionResult<RoleDto>> Post([FromBody] RolePostModel rolePostModel)
        {
            if (rolePostModel == null)
            {
                return BadRequest("Role cannot be null");
            }
            var roleDto = _mapper.Map<RoleDto>(rolePostModel);
            roleDto = await _roleService.AddRoleAsync(roleDto);
            if (roleDto == null)
            {
                return BadRequest("Added failed");
            }
            return CreatedAtAction(nameof(Get), new { id = roleDto.Id }, roleDto);
        }

        // PUT api/<RoleController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<RoleDto>> Put(int id, [FromBody] RolePostModel rolePostModel)
        {
            if (rolePostModel == null)
            {
                return BadRequest("Role cannot be null");
            }
            var roleDto = _mapper.Map<RoleDto>(rolePostModel);
            roleDto = await _roleService.UpdateRoleAsync(id, roleDto);
            if (roleDto == null)
            {
                return NotFound();
            }
            return Ok(roleDto);
        }

        // DELETE api/<RoleController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            RoleDto role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            await _roleService.DeleteRoleAsync(role);
            return NoContent();
        }
    }
}
