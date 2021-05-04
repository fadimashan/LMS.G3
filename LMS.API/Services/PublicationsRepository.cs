using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.API.Data;
using LMS.API.Models.Entities;
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
                .ToListAsync();
        }

        public async Task<IEnumerable<Publication>> GetAllWithAuthorsAsync()
        {
            return await _dbContext.Publications
                .Include(p => p.Subject)
                .Include(p => p.Type)
                .Include(p => p.Authors)
                .ToListAsync();
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
/* 
        public async void AddAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public async void UpdateAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public async void RemoveAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        } */

        public bool Any(int id)
        {
            return _dbContext.Publications.Any(p => p.Id == id);
        }
    }
}