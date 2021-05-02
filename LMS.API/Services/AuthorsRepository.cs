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
            throw new System.NotImplementedException();
        }

        public async Task<Author> GetAsync(int? id)
        {
            return await _dbContext.Authors.FindAsync(id);
        }

        public async Task<Author> GetWithPublicationsAsync(int? id)
        {
            throw new System.NotImplementedException();
        }

        public void Add(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public bool Any(int id)
        {
            return _dbContext.Authors.Any(a => a.Id == id);
        }

    }
}
