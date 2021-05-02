using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.API.Models.Entities;

namespace LMS.API.Services
{
    public interface IPublicationsRepository
    {
        Task<IEnumerable<Publication>> GetAllAsync();
        Task<IEnumerable<Publication>> GetAllWithAuthorsAsync();
        Task<Publication> GetAsync(int? id);
        Task<Publication> GetWithAuthorsAsync(int? id);
        void AddAsync(Publication publication);
        void UpdateAsync(Publication publication);
        void RemoveAsync(Publication publication);
    }
}