using ae_service_ship.Models;
using ae_service_ship.ViewModels;
using System.Collections.Generic;

namespace ae_service_ship.Repositories
{
    public interface IShipsRepository
    {
        //IEnumerable<PortDto> GetPorts();
        ShipDto GetShip(long id);
        IEnumerable<ShipDto> GetShips();
    }
}