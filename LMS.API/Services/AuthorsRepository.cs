using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LMS.API.Data;
using LMS.API.Models.Entities;

namespace LMS.API.Services
{
    public class AuthorsRepository : IAuthorsRepository
    {
        private readonly ApiDbContext _dbContext;

        public AuthorsRepository(ApiDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _dbContext.Authors.ToListAsync();
        }

        public async Task<IEnumerable<Author>> GetAllWithPublicationsAsync()
        {
            return await _dbContext.Authors
                .Include(a => a.Publications)
                .ThenInclude(p => p.Type)
                .Include(a => a.Publications)
                .ThenInclude(p => p.Subject)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Author>> GetAllWithPublicationsAsync(string nameLike)
        {
            if (string.IsNullOrWhiteSpace(nameLike))
            {
                return await GetAllWithPublicationsAsync();
            }

            nameLike = nameLike.Trim().ToLower();

            return await _dbContext.Authors
                .Include(a => a.Publications)
                .ThenInclude(p => p.Type)
                .Include(a => a.Publications)
                .ThenInclude(p => p.Subject)
                .Where(a => a.LastName.ToLower().Contains(nameLike) || 
                            a.FirstName.ToLower().Contains(nameLike))
                .ToListAsync();
        }

        public async Task<Author> GetAsync(int? id)
        {
            return await _dbContext.Authors
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Author> GetWithPublicationsAsync(int? id)
        {
            return await _dbContext.Authors
                .Include(a => a.Publications)
                .ThenInclude(p => p.Type)
                .Include(a => a.Publications)
                .ThenInclude(p => p.Subject)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Publication>> GetPublicationsAsync(int id)
        {
            var author = await _dbContext.Authors
                .Include(a => a.Publications)
                .ThenInclude(p => p.Type)
                .Include(a => a.Publications)
                .ThenInclude(p => p.Subject)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            return author.Publications;
        }

        public async Task<Publication> GetPublicationAsync(int authorId, int publicationId)
        {
            // Review: Do we need this?
            var author = await _dbContext.Authors
                .Include(a => a.Publications)
                .ThenInclude(p => p.Type)
                .Include(a => a.Publications)
                .ThenInclude(p => p.Subject)
                .FirstOrDefaultAsync(a => a.Id == authorId);
            
            return author.Publications.FirstOrDefault(p => p.Id == publicationId);
        }

        /* public async Task<Author> GetWithPublicationsAsync(int? id)
        {
            throw new System.NotImplementedException();
        } */
        
        public async Task AddAsync(Author author)
        {
            if (author is null)
            {
                throw new ArgumentNullException();
            }
            await _dbContext.AddAsync(author);
        }
/*
        public async void UpdateAsync(Publication publication)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(Author author)
        {
            _dbContext.Authors.Remove(author);
        }
*/

        public async Task<bool> SaveAsync()
        {
            return (await _dbContext.SaveChangesAsync()) >= 0;
        }
        
        public bool Exists(int id)
        {
            return _dbContext.Authors.Any(a => a.Id == id);
        }

    }
}
