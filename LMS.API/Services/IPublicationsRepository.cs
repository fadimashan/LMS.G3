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
        
        Task<Publication> GetAsync(int? id);
        Task<Publication> GetWithAuthorsAsync(int? id);

        Task<IEnumerable<Author>> GetAuthorsAsync(int id);
        Task<Author> GetAuthorAsync(int publicationId, int authorId); // Not sure that it's needed at all

        Task<IEnumerable<Subject>> GetAllSubjectsAsync();
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<Subject> GetSubjectByNameAsync(string typeName);
        Task<IEnumerable<PublicationType>> GetAllTypesAsync();
        Task<PublicationType> GetTypeByIdAsync(int id);
        Task<PublicationType> GetTypeByNameAsync(string typeName);
        
        Task AddPublicationAsync(Publication publication);
        Task AddSubjectAsync(Subject subject);
        Task AddTypeAsync(PublicationType type);
        // void UpdateAsync(Publication publication);
        Task RemovePublicationAsync(Publication publication);
        Task RemoveSubjectAsync(Subject subject);
        Task RemoveTypeAsync(PublicationType type);

        Task<bool> SaveAsync();
        
        public bool Exists(int id);
    }
}
