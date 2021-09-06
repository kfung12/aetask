using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ae_service_ship.Models;
using ae_service_ship.ViewModels;
using System.Data.Entity.SqlServer;
using System.Diagnostics;

namespace ae_service_ship.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipsController : ControllerBase
    {
        private readonly Models.AEDbContext _context;

        public ShipsController(Models.AEDbContext context)
        {
            _context = context;
        }

        // GET: api/Ships
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipDto>>> GetShips()
        {
            return await _context.Ships
                .Select(s => MaptoShipDto(s)).ToListAsync();
        }

        // PUT: api/Ships/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShip(long id, ShipDto shipDto)
        {
            if (id != shipDto.Id)
            {
                return BadRequest();
            }

            var ship = await _context.Ships.FindAsync(id);
            if (ship == null)
            {
                return NotFound();
            }

            //TODO: mapper
            ship.Name = shipDto.Name;
            ship.Lat = shipDto.Lat;
            ship.Long = shipDto.Long;
            ship.Velocity = shipDto.Velocity;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShipExists(id))
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

        // POST: api/Ships
        [HttpPost]
        public async Task<ActionResult<Ship>> CreateShip(ShipDto shipDto)
        {
            var ship = new Ship
            {
                Name = shipDto.Name,
                Lat = shipDto.Lat,
                Long = shipDto.Long,
                Velocity = shipDto.Velocity
            };


            _context.Ships.Add(ship);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShips), null);
        }

        [HttpGet]
        [Route("getClosestPort/{shipId}")]
        public async Task<ActionResult<PortInfoDto>> GetClosestPort(long shipId)
        {
            var ship = await _context.Ships.FindAsync(shipId);
            if (ship == null)
            {
                return NotFound();
            }
            else if (!ship.Velocity.HasValue)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Velocity information is missing");
            }

            //miles to let in deg = 3959
            //180/pi = 57.29
            var result = from p in _context.Ports
                         let distance = 3959 * Math.Acos(
                             Math.Sin((double)p.Lat / 57.29) * Math.Sin((double)ship.Lat / 57.29) +
                             Math.Cos((double)p.Lat / 57.29) * Math.Cos((double)ship.Lat / 57.29) *
                             Math.Cos((double)p.Long / 57.29 - (double)ship.Long / 57.29)
                             )
                         orderby distance
                         select new { p, distance };

            return await result.Select(r => new PortInfoDto()
            {
                Port = new PortDto() { Name = r.p.Name, Lat = r.p.Lat, Long = r.p.Long },
                Distance = (decimal)r.distance,
                ETA = (decimal)r.distance / ship.Velocity.Value
            }).FirstOrDefaultAsync();
        }


        private bool ShipExists(long id)
        {
            return _context.Ships.Any(e => e.Id == id);
        }

        private static ShipDto MaptoShipDto(Ship ship)
        {
            return new ShipDto
            {
                Id = ship.Id,
                Name = ship.Name,
                Lat = ship.Lat,
                Long = ship.Long,
                Velocity = ship.Velocity
            };
        }
        //private static double ConvertToRadian(decimal deg)
        //{
        //    return (double) deg * Math.PI / 180;
        //}
    }
}
