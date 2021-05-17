using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Data;

namespace LMS.Data.Repositories
{
    public class UoW : IUoW
    {
        private readonly LMSWebContext db;

        public IModulesRepository ModulesRepo { get; private set; }

        public UoW(LMSWebContext db)
        {
            this.db = db;
            ModulesRepo = new ModulesRepository(db);
        }

        public async Task CompleteAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
