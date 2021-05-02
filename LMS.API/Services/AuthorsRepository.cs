using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LMS.API.Data;
using LMS.API.Models.Entities;

namespace LMS.API.Services
{
    public class AuthorsRepository : IAuthorsRepository
    {
        private readonly ApiDbContext _dbContext;

        AuthorsRepository(ApiDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _dbContext.Authors.ToListAsync();
        }

        /*public async Task<IEnumerable<Author>> GetAllWithAuthorsAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Author> GetAsync(int? id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Author> GetWithAuthorsAsync(int? id)
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

        */
        
    }
}