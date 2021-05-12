using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.API.Models.Entities;

namespace LMS.API.Services
{
    public interface IAuthorsRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();
        Task<IEnumerable<Author>> GetAllWithPublicationsAsync();
        Task<IEnumerable<Author>> GetAllWithPublicationsAsync(string nameLike);
        Task<Author> GetAsync(int? id);
        Task<Author> GetByNameAsync(string name);
        Task<Author> GetWithPublicationsAsync(int? id);
        Task<IEnumerable<Publication>> GetPublicationsAsync(int id);
        Task<Publication> GetPublicationAsync(int authorId, int publicationId);
        
        Task AddAsync(Author author);
        // void UpdateAsync(Author author);
        Task RemoveAsync(Author author);

        Task<bool> SaveAsync();
        
        public bool Exists(int id);
        
    }
}
