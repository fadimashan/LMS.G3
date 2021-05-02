using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.API.Models.Entities;

namespace LMS.API.Services
{
    public interface IAuthorsRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();
        Task<IEnumerable<Author>> GetAllWithPublicationsAsync();
        Task<Author> GetAsync(int? id);
        Task<Author> GetWithPublicationsAsync(int? id);
        void Add(Publication publication);
        void Update(Publication publication);
        void Remove(Publication publication);
        public bool Any(int id);
    }
}