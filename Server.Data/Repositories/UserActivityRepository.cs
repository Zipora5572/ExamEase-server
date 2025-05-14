using Server.Core.Entities;
using Server.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data.Repositories
{
    public class UserActivityRepository : Repository<UserActivity>, IUserActivityRepository
    {
        readonly IDataContext _context;
        public UserActivityRepository(DataContext context) : base(context)
        {
            _context = context;
        }

      
    }
}
