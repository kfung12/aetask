using ae_service_ship.Models;
using ae_service_ship.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ae_service_ship.Repositories
{
    public interface IShipsRepository
    {
        Task<long> GetNewId();
        Task<IEnumerable<Ship>> GetShipsAsync();

        Task<Ship> GetShipAsync(long id);

        Task<Ship> CreateShipAsync(Ship ship);

        Task UpdateShipAsync(Ship ship);

        Task<PortInfoDto> GetClosestPortAsync(Ship ship);
    }
}