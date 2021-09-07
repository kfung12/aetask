using ae_service_ship.Models;
using ae_service_ship.Repositories;
using ae_service_ship.Utils;
using ae_service_ship.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ae_service_ship.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipsController : ControllerBase
    {

        private readonly IShipsRepository repository;

        public ShipsController(IShipsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipDto>>> GetShipsAsync()
        {
            return (await repository.GetShipsAsync()).Select(s => s.MaptoShipDto()).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShipDto>> GetShipAsync(long id)
        {
            var ship = await repository.GetShipAsync(id);
            if (ship == null)
            {
                return NotFound();
            }

            return ship.MaptoShipDto();
        }

        [HttpPost]
        public async Task<ActionResult<ShipDto>> CreateShipAsync(ShipDto shipDto)
        {
            long newId = await repository.GetNewId(); //TODO: read db auto increment

            var ship = new Ship
            {
                Id = newId,
                Name = shipDto.Name,
                Lat = shipDto.Lat,
                Long = shipDto.Long,
                Velocity = shipDto.Velocity
            };

            var createdShip = await repository.CreateShipAsync(ship);

            return CreatedAtAction(nameof(GetShipAsync), new { id = newId }, ship.MaptoShipDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipAsync(long id, ShipDto shipDto)
        {
            if (id != shipDto.Id)
            {
                return BadRequest();
            }

            var shipToUpdate = await repository.GetShipAsync(id);
            if (shipToUpdate == null)
            {
                return NotFound();
            }

            Ship ship = new Ship
            {
                Id = shipToUpdate.Id,
                Name = shipDto.Name,
                Long = shipDto.Long,
                Velocity = shipDto.Velocity
            };

            await repository.UpdateShipAsync(ship);

            return NoContent();
        }

        [HttpGet]
        [Route("getClosestPort/{shipId}")]
        public async Task<ActionResult<PortInfoDto>> GetClosestPortAsync(long shipId)
        {
            var ship = await repository.GetShipAsync(shipId);
            if (ship == null)
            {
                return NotFound();
            }
            else if (!ship.Velocity.HasValue || ship.Velocity <= 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Velocity information is missing or v<=0");
            }

            return await repository.GetClosestPortAsync(ship);
        }
    }
}
