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
                .ToListAsync();
        }

        public async Task<Author> GetAsync(int? id)
        {
            return await _dbContext.Authors
                .Include(a => a.Publications)
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

        /* public async Task<Author> GetWithPublicationsAsync(int? id)
        {
            throw new System.NotImplementedException();
        } */

        public void AddAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(int id)
        {
            return _dbContext.Authors.Any(a => a.Id == id);
        }

    }
}
