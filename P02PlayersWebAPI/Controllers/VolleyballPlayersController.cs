using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P02PlayersWebAPI.Models;

namespace P02PlayersWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolleyballPlayersController : ControllerBase
    {
        private readonly VolleyballWebContext _context;


        public VolleyballPlayersController(VolleyballWebContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetSize()
        {
            try
            {
                var totalCount = await _context.VolleyballPlayers.CountAsync();

                return Ok(new { TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                return Problem("Error occurred while retrieving the size data.", statusCode: 500);
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VolleyballPlayer>>> GetAllVolleyballPlayers()
        {
            try
            {
                var players = await _context.VolleyballPlayers.ToListAsync();
                return Ok(players);
            }
            catch (Exception ex) {
            return Problem("Error occurred while retrieving the Volleyball players.", statusCode: 500);
            }

        }



        // GET: api/VolleyballPlayers
        [HttpGet("{page}/{pageSize}")]
        public async Task<ActionResult<IEnumerable<VolleyballPlayer>>> GetVolleyballPlayers(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.VolleyballPlayers.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (page < 1 || page > totalPages)
            {
                return BadRequest("Invalid page number.");
            }

            var players = await _context.VolleyballPlayers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return players;
        }

        // GET: api/VolleyballPlayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VolleyballPlayer>> GetVolleyballPlayer(int id)
        {
          if (_context.VolleyballPlayers == null)
          {
              return NotFound();
          }
            var volleyballPlayer = await _context.VolleyballPlayers.FindAsync(id);

            if (volleyballPlayer == null)
            {
                return NotFound();
            }

            return volleyballPlayer;
        }

        // PUT: api/VolleyballPlayers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVolleyballPlayer(int id, VolleyballPlayer volleyballPlayer)
        {
            if (id != volleyballPlayer.Id)
            {
                return BadRequest();
            }

            _context.Entry(volleyballPlayer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VolleyballPlayerExists(id))
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

        // POST: api/VolleyballPlayers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VolleyballPlayer>> PostVolleyballPlayer(VolleyballPlayer volleyballPlayer)
        {
          if (_context.VolleyballPlayers == null)
          {
              return Problem("Entity set 'VolleyballWebContext.VolleyballPlayers'  is null.");
          }
            _context.VolleyballPlayers.Add(volleyballPlayer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVolleyballPlayer", new { id = volleyballPlayer.Id }, volleyballPlayer);
        }

        // DELETE: api/VolleyballPlayers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVolleyballPlayer(int id)
        {
            if (_context.VolleyballPlayers == null)
            {
                return NotFound();
            }
            var volleyballPlayer = await _context.VolleyballPlayers.FindAsync(id);
            if (volleyballPlayer == null)
            {
                return NotFound();
            }

            _context.VolleyballPlayers.Remove(volleyballPlayer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VolleyballPlayerExists(int id)
        {
            return (_context.VolleyballPlayers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
