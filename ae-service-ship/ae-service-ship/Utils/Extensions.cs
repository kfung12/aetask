using ae_service_ship.Models;
using ae_service_ship.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ae_service_ship.Utils
{
    public static class Extensions
    {
        public static ShipDto MaptoShipDto(this Ship ship)
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
    }
}
