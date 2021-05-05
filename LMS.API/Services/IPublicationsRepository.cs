using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.API.Models.Entities;

namespace LMS.API.Services
{
    public interface IPublicationsRepository
    {
        Task<IEnumerable<Publication>> GetAllAsync();
        Task<IEnumerable<Publication>> GetAllWithAuthorsAsync();
        Task<IEnumerable<Publication>> GetAllAsync(string subject);
        Task<Publication> GetAsync(int? id);
        Task<Publication> GetWithAuthorsAsync(int? id);

        Task<IEnumerable<Author>> GetAuthorsAsync(int id);
        Task<Author> GetAuthorAsync(int publicationId, int authorId);
        // void AddAsync(Publication publication);
        // void UpdateAsync(Publication publication);
        // void RemoveAsync(Publication publication);
        public bool Any(int id);
    }
}