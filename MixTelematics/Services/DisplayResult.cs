using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixTelematics.Services
{
    public class DisplayResult
    {
        public static void Display(Co_Ordinates targetCoordinate, int vehicleId) 
        {
            Console.WriteLine($"Co-Ordinates : ({targetCoordinate.Latitude},{targetCoordinate.Longitude})");
            Console.WriteLine($"Closest Vehicle ID: {vehicleId}");
            Console.WriteLine();
        }

    }
}
