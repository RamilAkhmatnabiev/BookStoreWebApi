using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreWebApi.Models;

namespace BookStoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly BookStoresDbContext _context;

        public PublishersController(BookStoresDbContext context)
        {
            _context = context;
        }

        // GET: api/Publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublishers()
        {
            if (_context.Publishers == null)
            {
                return NotFound();
            }

            return await _context.Publishers.ToListAsync();
        }

        // GET: api/Publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            if (_context.Publishers == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers.FindAsync(id);

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // GET: api/Publishers/GetPublisherWithDetails/5
        [HttpGet("GetPublisherWithDetails/{id}")]
        public async Task<ActionResult<Publisher>> GetPublisherWithDetails(int id)
        {
            if (_context.Publishers == null)
            {
                return NotFound();
            }

            //EXAMPLE OF EAGLE LODING
            var publisher = _context.Publishers
                .Include(x => x.Users)
                .ThenInclude(x => x.Role)
                .Include(x => x.Books)
                .ThenInclude(x => x.Sales)
                .FirstOrDefault(x => x.PubId == id);

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // GET: api/Publishers/GetPublisherWithDetailsExplicit/5
        [HttpGet("GetPublisherWithDetailsExplicit/{id}")]
        public async Task<ActionResult<Publisher>> GetPublisherWithDetailsExplicit(int id)
        {
            if (_context.Publishers == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers.SingleAsync(x => x.PubId == id);

            if (publisher == null)
            {
                return NotFound();
            }
            
            //Before this string the publisher does not contain the Users and Book collection, cause his come in 'Lazy' mode

            //Include the users collection
            await _context.Entry(publisher)
                .Collection(x => x.Users)
                .Query()
                .LoadAsync();

            //Include the book => sales => store
            await _context.Entry(publisher)
                .Collection(x => x.Books)
                .Query()
                .Include(x => x.Sales)
                .ThenInclude(x => x.Store)
                .LoadAsync();

            return publisher;
        }

        // PUT: api/Publishers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublisher(int id, Publisher publisher)
        {
            if (id != publisher.PubId)
            {
                return BadRequest();
            }

            _context.Entry(publisher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(id))
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

        // POST: api/Publishers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Publisher>> PostPublisher(Publisher publisher)
        {
            if (_context.Publishers == null)
            {
                return Problem("Entity set 'BookStoresDbContext.Publishers'  is null.");
            }

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublisher", new { id = publisher.PubId }, publisher);
        }

        // DELETE: api/Publishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            if (_context.Publishers == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublisherExists(int id)
        {
            return (_context.Publishers?.Any(e => e.PubId == id)).GetValueOrDefault();
        }
    }
}