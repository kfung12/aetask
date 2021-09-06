using ae_service_ship.Models;
using ae_service_ship.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ae_service_ship.Utils;

namespace ae_service_ship.Repositories
{
    public class InMemoryShipsRepository : IShipsRepository
    {
        private readonly List<Port> ports = new List<Port>()
        {
            new Port { Id = 1, Name = "Port1", Lat = 50, Long = 50},
            new Port { Id = 2, Name = "Port2", Lat = 7.900000M, Long = 126.070000M},
            new Port { Id = 3, Name = "Port3", Lat = -30.140000M, Long = -9.130000M},
            new Port { Id = 4, Name = "Port4", Lat = 53.130000M, Long = 28.100000M},
            new Port { Id = 5, Name = "Port5", Lat = 52.700000M, Long = 7.810000M}
        };

        private readonly List<Ship> ships = new List<Ship>()
        {
            new Ship { Id = 1, Name = "Ship1", Lat = 0.120000M, Long = 0.120000M, Velocity = 50.00M},
            new Ship { Id = 2, Name = "Ship2", Lat = 50.000000M, Long = 50.000000M, Velocity = 5000.00M},
            new Ship { Id = 3, Name = "Ship3", Lat = -2.590000M, Long = 133.340000M},
            new Ship { Id = 4, Name = "Ship4", Lat = -41.010000M, Long = 86.760000M},
            new Ship { Id = 5, Name = "Ship5", Lat = -5.080000M, Long = 10.540000M}
        };

        public IEnumerable<ShipDto> GetShips()
        {
            return ships.Select(s => s.MaptoShipDto());
        }

        public ShipDto GetShip(long id)
        {
            return ships.Where(s => s.Id == id).Select(s => s.MaptoShipDto()).SingleOrDefault();
        }

        //public IEnumerable<Port> GetPorts()
        //{
        //    return ports;
        //}
        public Ship

    }
}
