using ae_service_ship.Models;
using ae_service_ship.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Ship>> GetShipsAsync()
        {
            return await Task.FromResult(ships);
        }

        public async Task<Ship> GetShipAsync(long id)
        {
            var ship = ships.Where(s => s.Id == id).FirstOrDefault();
            return await Task.FromResult(ship);
        }

        public async Task<Ship> CreateShipAsync(Ship ship)
        {
            ships.Add(ship);
            return await Task.FromResult(ship);
        }

        public async Task UpdateShipAsync(Ship ship)
        {
            var idx = ships.FindIndex(s => s.Id == ship.Id);
            ships[idx] = ship;
            await Task.CompletedTask;
        }

        public async Task<PortInfoDto> GetClosestPortAsync(Ship ship)
        {
            //var ship = ships.Where(s => s.Id == shipId).SingleOrDefault();
            //if (ship == null || !ship.Velocity.HasValue || ship.Velocity.Value == 0)
            //{
            //    return null;
            //}

            //miles to let in deg = 3959
            //180/pi = 57.29
            var query = from p in ports
                         let distance = 3959 * Math.Acos(
                             Math.Sin((double)p.Lat / 57.29) * Math.Sin((double)ship.Lat / 57.29) +
                             Math.Cos((double)p.Lat / 57.29) * Math.Cos((double)ship.Lat / 57.29) *
                             Math.Cos((double)p.Long / 57.29 - (double)ship.Long / 57.29)
                             )
                         orderby distance
                         select new { p, distance };

            var result = query.Select(r => new PortInfoDto()
            {
                Port = new PortDto() { Name = r.p.Name, Lat = r.p.Lat, Long = r.p.Long },
                Distance = (decimal)r.distance,
                ETA = (decimal)r.distance / ship.Velocity.Value
            }).FirstOrDefault();

            return await Task.FromResult(result);
        }

        public async Task<long> GetNewId()
        {
            return await Task.FromResult(ships.Max(s => s.Id) + 1);
        }

        //private static double ConvertToRadian(decimal deg)
        //{
        //    return (double)deg * Math.PI / 180;
        //}
    }
}
