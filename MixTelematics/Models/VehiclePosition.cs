using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixTelematics.Models
{
    public class VehiclePosition
    {
        public int VehicleId { get; set; }
        public string VehicleRegistration { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public ulong RecordedTimeUTC { get; set; }

        public const int SizeInBytes= 2000001;
    }
}
