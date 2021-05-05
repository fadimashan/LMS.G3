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
        Task<IEnumerable<Publication>> GetPublicationsAsync(int id);
        Task<Publication> GetPublicationAsync(int authorId, int publicationId);
        void AddAsync(Publication publication);
        void UpdateAsync(Publication publication);
        void RemoveAsync(Publication publication);
        public bool Exists(int id);
        
    }
}