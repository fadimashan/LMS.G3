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
using LMS.API.DtoModels;
using LMS.API.Helpers;
using AutoMapper;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;
        private readonly IAuthorsRepository _authorsRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ApiDbContext context, IAuthorsRepository authorsRepository,
            IMapper mapper)
        {
            _dbContext = context;
            _authorsRepository = authorsRepository ?? throw new ArgumentNullException(nameof(authorsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors(string searchQuery,string sortQuery)
        {
            var authorsFromRepo = await _authorsRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var authorFromRepo = await _authorsRepository.GetAsync(id);

            if (authorFromRepo is null)
            {
                return NotFound();
            }

            return new JsonResult(authorFromRepo);
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

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
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

        private bool AuthorExists(int id)
        {
            return _dbContext.Authors.Any(e => e.Id == id);
        }
    }
}
