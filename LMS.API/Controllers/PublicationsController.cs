using System;
using System.Collections.Generic;
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
        private readonly IAuthorsRepository _authorsRepository;
        private readonly IMapper _mapper;

        // ToDo: To be removed when all methods are using the Repository
        private readonly ApiDbContext _dbContext;
        
        public PublicationsController(ApiDbContext context, IPublicationsRepository publicationsRepository, IAuthorsRepository authorsRepository, IMapper mapper)
        {
            _dbContext = context;
            _publicationsRepository = publicationsRepository ?? throw new ArgumentNullException(nameof(publicationsRepository));
            _authorsRepository = authorsRepository ?? throw new ArgumentNullException(nameof(mapper));
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
        [HttpGet("{id}", Name = "GetPublication")]
        public async Task<ActionResult<PublicationWithAuthorsDto>> GetPublication(int id)
        {
            var publicationFromRepo = await _publicationsRepository.GetWithAuthorsAsync(id);

            if (publicationFromRepo is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PublicationWithAuthorsDto>(publicationFromRepo));
        }
        
        [HttpGet("{id}/authors", Name = "GetAuthorsForPublication")]
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
        [HttpGet("{publicationId}/authors/{authorId}", Name = "GetAuthorForPublication")]
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

        [HttpGet("subjects")]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects()
        {
            var subjects = await _publicationsRepository.GetAllSubjectsAsync();

            return Ok(_mapper.Map<IEnumerable<SubjectDto>>(subjects));
        }

        [HttpGet("subjects/{id}", Name = "GetSubject")]
        public async Task<ActionResult<SubjectDto>> GetSubject(int id)
        {
            var subject = await _publicationsRepository.GetSubjectByIdAsync(id);
            return Ok(_mapper.Map<SubjectDto>(subject));
        }
        
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<PublicationTypeDto>>> GetTypes()
        {
            var types = await _publicationsRepository.GetAllTypesAsync();

            return Ok(_mapper.Map<IEnumerable<PublicationTypeDto>>(types));
        }

        [HttpGet("types/{id}", Name = "GetType")]
        public async Task<ActionResult<SubjectDto>> GetType(int id)
        {
            var types = await _publicationsRepository.GetTypeByIdAsync(id);
            return Ok(_mapper.Map<PublicationTypeDto>(types));
        }
        
        // POST: api/Publications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PublicationWithAuthorsDto>> CreatePublication(PublicationCreationDto newPublication)
        {
            // Map the DTO to the Publication Entity
            var publicationEntity = _mapper.Map<Publication>(newPublication);
            
            // For each AuthorId find the corresponding author
            // and if it's not null -- add it
            publicationEntity.Authors = new List<Author>();
            foreach (var authorId in newPublication.AuthorIds)
            {
                var authorFromRepo = await _authorsRepository.GetAsync(authorId);
                if (authorFromRepo is not null)
                {
                    publicationEntity.Authors.Add(authorFromRepo);
                }
            }
            // Add the Publication entity to the DB
            await _publicationsRepository.AddPublicationAsync(publicationEntity);
            // Save the changes to the DB
            await _publicationsRepository.SaveAsync();
            
            // Return the corresponding DTO
            var publicationToReturn = _mapper.Map<PublicationWithAuthorsDto>(publicationEntity);
            // OLD: return CreatedAtRoute("GetPublication", new {id = publicationToReturn.Id}, publicationToReturn);
            return CreatedAtAction("GetPublication", new { id = publicationToReturn.Id }, publicationToReturn);
        }

        [HttpPost("subjects")]
        public async Task<ActionResult<SubjectDto>> CreateSubject(SubjectDto newSubject)
        {
            if (newSubject.Id is not null)
            {
                return BadRequest();
            }

            var subjectEntity = _mapper.Map<Subject>(newSubject);
            await _publicationsRepository.AddSubjectAsync(subjectEntity);
            await _publicationsRepository.SaveAsync();
            var subjectToReturn = _mapper.Map<SubjectDto>(subjectEntity);
            return CreatedAtAction("GetSubject", new {id = subjectToReturn.Id}, subjectToReturn);
        }
        
        [HttpPost("types")]
        public async Task<ActionResult<SubjectDto>> CreateType(PublicationTypeDto newType)
        {
            if (newType.Id is not null)
            {
                return BadRequest();
            }

            var typeEntity = _mapper.Map<PublicationType>(newType);
            await _publicationsRepository.AddTypeAsync(typeEntity);
            await _publicationsRepository.SaveAsync();
            var publicationTypeToReturn = _mapper.Map<PublicationTypeDto>(typeEntity);
            return CreatedAtAction("GetType", new {id = publicationTypeToReturn.Id}, publicationTypeToReturn);
        }

        // FIXME: The code from this method should probably be moved to another class
        // and then called from both this controller and the AuthorsController 
        [HttpPost("{publicationId}/authors")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> CreateAuthorsForPublication(int publicationId, IEnumerable<AuthorCreationDto> newAuthors)
        {
            if (!_publicationsRepository.Exists(publicationId))
            {
                return NotFound();
            }

            var publicationEntity = await _publicationsRepository.GetWithAuthorsAsync(publicationId);
            var authors = _mapper.Map<IEnumerable<Author>>(newAuthors);
            foreach (var author in authors)
            {
                await _authorsRepository.AddAsync(author);
                publicationEntity.Authors.Add(author);
            }

            await _publicationsRepository.SaveAsync();

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authors);
            
            return CreatedAtAction("GetAuthorsForPublication", new { id = publicationId }, authorsToReturn);
        }

        /*[HttpPost("{publicationId}/authors")]
        public async Task<ActionResult<AuthorDto>> CreateAuthorForPublication(int publicationId, AuthorCreationDto newAuthor)
        {
            if (!_publicationsRepository.Exists(publicationId))
            {
                return NotFound();
            }

            var publicationEntity = await _publicationsRepository.GetWithAuthorsAsync(publicationId);
            var authorEntity = _mapper.Map<Author>(newAuthor);
            await _authorsRepository.AddAsync(authorEntity);
            publicationEntity.Authors.Add(authorEntity);
            
            await _publicationsRepository.SaveAsync();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            
            return CreatedAtAction("GetAuthorForPublication", new { publicationId, authorId = authorToReturn.Id }, authorToReturn);
        }*/

        
/* FIXME: Up to this point everything works properly as expected.
 The rest of the methods work too but must be refactored to utilise Repositories and DTOs*/

        // PUT: api/Publications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<PublicationDto>> UpdatePublication(int id, PublicationCreationDto publicationDto)
        {
            var publicationFromRepo = await _publicationsRepository.GetAsync(id);

            if (publicationFromRepo is null)
            {
                return NotFound();
            }

            _mapper.Map(publicationDto, publicationFromRepo);

            if (await _publicationsRepository.SaveAsync())
            {
                return Ok(_mapper.Map<PublicationDto>(publicationFromRepo));
            }
            else
            {
                return StatusCode(500);
            }
        }

/*         
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
                // await _dbContext.SaveChangesAsync();
                await _publicationsRepository.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_publicationsRepository.Exists(id))
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
 */

        // DELETE: api/Publications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublication(int id)
        {
            // var publication = await _dbContext.Publications.FindAsync(id);
            var publication = await _publicationsRepository.GetAsync(id);
            if (publication == null)
            {
                return NotFound();
            }

            // _dbContext.Publications.Remove(publication);
            await _publicationsRepository.RemovePublicationAsync(publication);
            // await _dbContext.SaveChangesAsync();
            // await _publicationsRepository.SaveAsync();

            return NoContent();
        }
        
        [HttpDelete("subjects/{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _publicationsRepository.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            await _publicationsRepository.RemoveSubjectAsync(subject);
            
            return NoContent();
        }

        [HttpDelete("types/{id}")]
        public async Task<IActionResult> DeleteType(int id)
        {
            var publicationType = await _publicationsRepository.GetTypeByIdAsync(id);
            if (publicationType == null)
            {
                return NotFound();
            }

            await _publicationsRepository.RemoveTypeAsync(publicationType);
            
            return NoContent();
        }

        // This method will be removed when PUT is implemented properly
        /*private bool PublicationExists(int id)
        {
            return _dbContext.Publications.Any(e => e.Id == id);
        }*/
    }
}
