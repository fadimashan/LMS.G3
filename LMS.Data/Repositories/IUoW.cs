using System.Threading.Tasks;

namespace LMS.Data.Repositories
{
    public interface IUoW
    {
        IModulesRepository ModulesRepo { get; }

        Task CompleteAsync();
    }
}