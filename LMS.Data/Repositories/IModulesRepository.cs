using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Core.Entities;

namespace LMS.Data.Repositories
{
   public interface IModulesRepository
    {
        Task<IEnumerable<Module>> GetAll();
    }
}