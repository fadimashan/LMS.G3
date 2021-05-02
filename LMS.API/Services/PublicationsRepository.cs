using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.API.Data;
using LMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Services
{
    public class PublicationsRepository : IPublicationsRepository
    {
        private readonly ApiDbContext _dbContext;

        PublicationsRepository(ApiDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<Publication>> GetAllAsync()
        {
            return await _dbContext.Publications.ToListAsync();
        }

        /*
        public async Task<IEnumerable<Publication>> GetAllWithAuthorsAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Publication> GetAsync(int? id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Publication> GetWithAuthorsAsync(int? id)
        {
            throw new System.NotImplementedException();
        }

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
        }*/
        
    }
}