using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ae_service_ship.Models
{
    public class Port
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
    }
}
