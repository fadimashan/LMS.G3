using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.API.Models.Entities;

namespace LMS.API.Services
{
    public interface IAuthorsRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();
        /*Task<IEnumerable<Author>> GetAllWithAuthorsAsync();
        Task<Author> GetAsync(int? id);
        Task<Author> GetWithAuthorsAsync(int? id);
        void Add(Publication publication);
        void Update(Publication publication);
        void Remove(Publication publication);*/
    }
}