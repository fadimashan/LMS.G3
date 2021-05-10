using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.API.Data;
using LMS.API.Models.Entities;
using LMS.API.Services;
using AutoMapper;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/publications")]
    public class PublicationsController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;
        private readonly IPublicationsRepository _publicationsRepository;
        private readonly IMapper _mapper;

        public PublicationsController(ApiDbContext context, IPublicationsRepository publicationsRepository)
        {
            _dbContext = context;
            _publicationsRepository = publicationsRepository ?? 
                throw new ArgumentNullException(nameof(publicationsRepository));
            //_mapper = mapper ?? 
            //    throw new ArgumentNullException(nameof(mapper));

        }

        // GET: api/Publications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publication>>> GetPublications()
        {
            var publicationsFromRepo = await _publicationsRepository.GetAllAsync();
            return Ok(publicationsFromRepo);
        }

        // GET: api/Publications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Publication>> GetPublication(int id)
        {
            var publicationFromRepo = await _publicationsRepository.GetAsync(id);

            if (publicationFromRepo is null)
            {
                return NotFound();
            }

            return Ok(publicationFromRepo);
        }

        // PUT: api/Publications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublication(int id, Publication publication)
        {
            //if (id != publication.Id)
            //{
            //    return BadRequest();
            //}

            var publicationFromRepo = await _publicationsRepository.GetAsync(id);

            if (publicationFromRepo is null) return StatusCode(StatusCodes.Status404NotFound);

            //_dbContext.Entry(publication).State = EntityState.Modified;
            publicationFromRepo = new Publication()
            {
                Id = publication.Id,
                Title = publication.Title,
            };

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublicationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Publications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Publication>> PostPublication(Publication publication)
        {
            _dbContext.Publications.Add(publication);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetPublication", new { id = publication.Id }, publication);
        }

        // DELETE: api/Publications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublication(int id)
        {
            var publication = await _dbContext.Publications.FindAsync(id);
            if (publication == null)
            {
                return NotFound();
            }

            _dbContext.Publications.Remove(publication);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool PublicationExists(int id)
        {
            return _dbContext.Publications.Any(e => e.Id == id);
        }
    }
}
