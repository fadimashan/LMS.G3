using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using LMS.API.Data;
using LMS.API.Models.DTO;
using LMS.API.Models.Entities;
using LMS.API.ResourceParameters;
using LMS.API.Services;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/publications")]
    public class PublicationsController : ControllerBase
    {
        private readonly IPublicationsRepository _publicationsRepository;
        private readonly IMapper _mapper;

        // ToDo: To be removed when all methods are using the Repository
        private readonly ApiDbContext _dbContext;
        

        public PublicationsController(ApiDbContext context, IPublicationsRepository publicationsRepository, IMapper mapper)
        {
            _dbContext = context;
            _publicationsRepository = publicationsRepository ?? throw new ArgumentNullException(nameof(publicationsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/Publications
        [HttpGet]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<PublicationWithAuthorsDto>>> GetPublications([FromQuery] PublicationsResourceParameters searchParameters)
        {
            var publicationsFromRepo = await _publicationsRepository.GetAllAsync(searchParameters);
            
            return Ok(_mapper.Map<IEnumerable<PublicationWithAuthorsDto>>(publicationsFromRepo));
        }

        // GET: api/Publications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PublicationWithAuthorsDto>> GetPublication(int id)
        {
            var publicationFromRepo = await _publicationsRepository.GetWithAuthorsAsync(id);

            if (publicationFromRepo is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PublicationWithAuthorsDto>(publicationFromRepo));
        }
        
        [HttpGet("{id}/authors")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthorsForPublication(int id)
        {
            if (!_publicationsRepository.Exists(id))
            {
                return NotFound();
            }
            
            var authorsFromRepo = await _publicationsRepository.GetAuthorsAsync(id);

            if (authorsFromRepo is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }
        
        // REVIEW: Do we need this?
        [HttpGet("{publicationId}/authors/{authorId}")]
        public async Task<ActionResult<AuthorDto>> GetAuthorForPublication(int publicationId, int authorId)
        {
            if (!_publicationsRepository.Exists(publicationId))
            {
                return NotFound();
            }
            
            var authorForPublicationFromRepo = await _publicationsRepository.GetAuthorAsync(publicationId, authorId);

            if (authorForPublicationFromRepo is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(authorForPublicationFromRepo));
        }

/* FIXME: Up to this point everything works properly as expected.
 The rest of the methods work too but must be refactored to utilise Repositories and DTOs*/

        // POST: api/Publications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Publication>> PostPublication(Publication publication)
        {
            _dbContext.Publications.Add(publication);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetPublication", new { id = publication.Id }, publication);
        }
        
        // PUT: api/Publications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublication(int id, Publication publication)
        {
            if (id != publication.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(publication).State = EntityState.Modified;

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

        // This method will be removed when PUT is implemented properly
        private bool PublicationExists(int id)
        {
            return _dbContext.Publications.Any(e => e.Id == id);
        }
    }
}
