using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Entities;
using LMS.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data.Repositories
{
    public class ModulesRepository : IModulesRepository
    {
        private readonly LMSWebContext db;

        public ModulesRepository(LMSWebContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<Module>> GetAll()
        {
            return await db.Module.ToListAsync();
        }
    }
}
