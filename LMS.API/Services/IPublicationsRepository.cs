using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.API.Models.Entities;
using LMS.API.ResourceParameters;

namespace LMS.API.Services
{
    public interface IPublicationsRepository
    {
        Task<IEnumerable<Publication>> GetAllAsync();
        Task<IEnumerable<Publication>> GetAllAsync(PublicationsResourceParameters searchParameters);
        Task<IEnumerable<Publication>> GetAllWithAuthorsAsync();
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