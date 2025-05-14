using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data.Repositories
{
    public class UserRepository:Repository<User>,IUserRepository
    {

        readonly DataContext _context;
        readonly IMapper _mapper;
        public UserRepository(DataContext context,IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {          
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }
            var userDto = _mapper.Map<UserDto>(user);
          
            return userDto;
        }

    }
}
