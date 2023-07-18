using System.Diagnostics;

namespace MixTelematics.Services
{
    public class VehicleFinder
    {
        public static void FindVehicle()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string fileName = @"VehiclePositions.dat";
                var vehiclePositions = BinaryFIleREader.ReadBinaryDataFile(fileName);



                var quadTree = new NearestVehicleFinder(vehiclePositions);
                //quadTree.Build(vehiclePositions);

                var targetCoordinates = new[]
                {
            new { Latitude = 34.544909f, Longitude = -102.100843f },
            new { Latitude = 32.345544f, Longitude = -99.123124f },
            new { Latitude = 33.234235f, Longitude = -100.214124f },
            new { Latitude = 35.195739f, Longitude = -95.348899f },
            new { Latitude = 31.895839f, Longitude = -97.789573f },
            new { Latitude = 32.895839f, Longitude = -101.789573f },
            new { Latitude = 34.115839f, Longitude = -100.225732f },
            new { Latitude = 32.335839f, Longitude = -99.992232f },
            new { Latitude = 33.535339f, Longitude = -94.792232f },
            new { Latitude = 32.234235f, Longitude = -100.222222f }
        };

                foreach (var targetCoordinate in targetCoordinates)
                {
                    var nearestVehicle = quadTree.FindNearest(targetCoordinate.Latitude, targetCoordinate.Longitude);

                    Console.WriteLine($"Position #{Array.IndexOf(targetCoordinates, targetCoordinate) + 1}:");
                    Console.WriteLine($"Latitude: {targetCoordinate.Latitude}");
                    Console.WriteLine($"Longitude: {targetCoordinate.Longitude}");
                    if (nearestVehicle != null)
                    {
                        Console.WriteLine($"Closest Vehicle ID: {nearestVehicle.VehicleId}");
                    }
                    else
                    {
                        Console.WriteLine("No closest vehicle found.");
                    }

                    Console.WriteLine();
                }
                stopwatch.Stop();
                Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An Error Occured: {ex.Message}");
            }

        }

    }
}
