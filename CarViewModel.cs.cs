using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarManagement
{
    
    
        public class CarViewModel
        {
            public int Id { get; set; }
            public string NumberPlate { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public int Year { get; set; }
            public string EngineType { get; set; }
            public double EngineVolume { get; set; }
            public int Kilowatts { get; set; }
        }
    
}
