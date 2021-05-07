using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.API.Data;
using LMS.API.Models.Entities;
using LMS.API.ResourceParameters;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Services
{
    public class PublicationsRepository : IPublicationsRepository
    {
        private readonly ApiDbContext _dbContext;

        public PublicationsRepository(ApiDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Publication>> GetAllAsync()
        {
            return await _dbContext.Publications
                .Include(p => p.Subject)
                .Include(p => p.Type)
                .Include(p => p.Authors)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Publication>> GetAllAsync(PublicationsResourceParameters searchParameters)
        {
            if (searchParameters is null)
            {
                throw new ArgumentNullException();
            }
            
            if (string.IsNullOrWhiteSpace(searchParameters.Subject) && string.IsNullOrWhiteSpace(searchParameters.SearchQuery))
            {
                return await GetAllAsync();
            }

            var publications = _dbContext.Publications
                .Include(p => p.Subject)
                .Include(p => p.Type)
                .Include(p => p.Authors)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchParameters.Subject))
            {
                var subject = searchParameters.Subject.Trim();
                publications = publications.Where(p => p.Subject.Name.ToLower().Equals(subject.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(searchParameters.SearchQuery))
            {
                var searchQuery = searchParameters.SearchQuery.Trim();
                publications = publications.Where(p => p.Title.ToLower().Contains(searchQuery.ToLower()));
            }

            return await publications.ToListAsync();
        }

        public async Task<Publication> GetAsync(int? id)
        {
            return await _dbContext.Publications
                .Include(p => p.Type)
                .Include(p => p.Subject)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Publication> GetWithAuthorsAsync(int? id)
        {
            return await _dbContext.Publications
                .Include(p => p.Type)
                .Include(p => p.Subject)
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync(int id)
        {
            var publication = await _dbContext.Publications
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            return publication.Authors;
        }

        public async Task<Author> GetAuthorAsync(int publicationId, int authorId)
        {
            var publication = await _dbContext.Publications
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(p => p.Id == publicationId);

            return publication.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public async Task<PublicationType> GetTypeByIdAsync(int id)
        {
            return await _dbContext.PublicationTypes.FirstOrDefaultAsync(t => t.Id == id);
        }
        
        public async Task<PublicationType> GetTypeByNameAsync(string typeName)
        {
            return await _dbContext.PublicationTypes.FirstOrDefaultAsync(t => t.Name.ToLower().Equals(typeName.ToLower()));
        }
        
        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            return await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == id);
        }
        
        public async Task<Subject> GetSubjectByNameAsync(string typeName)
        {
            return await _dbContext.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower().Equals(typeName.ToLower()));
        }
        
        public async Task AddAsync(Publication publication)
        {
            if (publication is null)
            {
                throw new ArgumentNullException();
            }
            
            await _dbContext.AddAsync(publication);
        }

        /* 
        public async void UpdateAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public async void RemoveAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        } */

        public async Task<bool> SaveAsync()
        {
            return (await _dbContext.SaveChangesAsync()) >= 0;
        }
        
        public bool Exists(int id)
        {
            return _dbContext.Publications.Any(p => p.Id == id);
        }
    }
}
