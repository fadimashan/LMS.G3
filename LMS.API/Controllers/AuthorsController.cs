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
using LMS.API.Services;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorsRepository _authorsRepository;
        private readonly IMapper _mapper;
        
        // ToDo: To be removed when all methods are using the Repository
        private readonly ApiDbContext _dbContext;

        public AuthorsController(ApiDbContext context, IAuthorsRepository authorsRepository, IMapper mapper)
        {
            _dbContext = context;
            _authorsRepository = authorsRepository ?? throw new ArgumentNullException(nameof(authorsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/Authors
        [HttpGet]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<AuthorWithPublicationsDto>>> GetAuthors(string nameLike)
        {
            var authorsFromRepo = await _authorsRepository.GetAllWithPublicationsAsync(nameLike);

            return Ok(_mapper.Map<IEnumerable<AuthorWithPublicationsDto>>(authorsFromRepo));
        }

        // GET: api/Authors/5
        [HttpGet("{id}", Name = "GetAuthor")]
        public async Task<ActionResult<AuthorWithPublicationsDto>> GetAuthor(int id)
        {
            var authorFromRepo = await _authorsRepository.GetWithPublicationsAsync(id);

            if (authorFromRepo is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorWithPublicationsDto>(authorFromRepo));
        }

        [HttpGet("{id}/publications")]
        public async Task<ActionResult<IEnumerable<PublicationDto>>> GetPublicationsForAuthor(int id)
        {
            if (!_authorsRepository.Exists(id))
            {
                return NotFound();
            }
            
            var publicationsFromRepo = await _authorsRepository.GetPublicationsAsync(id);

            return Ok(_mapper.Map<IEnumerable<PublicationDto>>(publicationsFromRepo));
        }
        
        // Review: Do we need this?
        [HttpGet("{authorId}/publications/{publicationId}")]
        public async Task<ActionResult<PublicationDto>> GetPublicationForAuthor(int authorId, int publicationId)
        {
            if (!_authorsRepository.Exists(authorId))
            {
                return NotFound();
            }
            
            var publicationForAuthorFromRepo = await _authorsRepository.GetPublicationAsync(authorId, publicationId);

            if (publicationForAuthorFromRepo is null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<PublicationDto>(publicationForAuthorFromRepo));
        }

/* FIXME: Up to this point everything works properly as expected.
 The rest of the methods work too but must be refactored to utilise Repositories and DTOs*/

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorCreationDto author)
        {
            var authorEntity = _mapper.Map<Author>(author);
            // _dbContext.Authors.Add(author);
            await _authorsRepository.AddAsync(authorEntity);
            // await _dbContext.SaveChangesAsync();
            await _authorsRepository.SaveAsync();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            
            return CreatedAtAction("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);
        }
        
        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(author).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _dbContext.Authors.Remove(author);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // This method will be removed when PUT is implemented properly
        private bool AuthorExists(int id)
        {
            return _dbContext.Authors.Any(e => e.Id == id);
        }
    }
}
