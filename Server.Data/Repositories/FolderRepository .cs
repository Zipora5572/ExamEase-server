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
    public class FolderRepository : Repository<Folder>, IFolderRepository
    {
        readonly IDataContext _context;
        public FolderRepository(DataContext context) : base(context)
        {
            _context = context;
        }

      
    }
}
