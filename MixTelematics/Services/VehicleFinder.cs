using System.Diagnostics;

namespace MixTelematics.Services
{
    public class VehicleFinder
    {
        public static void FindVehicle()
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string fileName = @"VehiclePositions.dat";
            var vehiclePositions = BinaryFIleREader.ReadBinaryDataFile(fileName);



            var quadTree = new NearestVehicleFinder(vehiclePositions);
            //quadTree.Build(vehiclePositions);

            var targetCoordinates = new Co_Ordinates[]
            {
            new Co_Ordinates { Latitude = 34.544909f, Longitude = -102.100843f },
            new Co_Ordinates { Latitude = 32.345544f, Longitude = -99.123124f },
            new Co_Ordinates { Latitude = 33.234235f, Longitude = -100.214124f },
            new Co_Ordinates { Latitude = 35.195739f, Longitude = -95.348899f },
            new Co_Ordinates { Latitude = 31.895839f, Longitude = -97.789573f },
            new Co_Ordinates { Latitude = 32.895839f, Longitude = -101.789573f },
            new Co_Ordinates { Latitude = 34.115839f, Longitude = -100.225732f },
            new Co_Ordinates { Latitude = 32.335839f, Longitude = -99.992232f },
            new Co_Ordinates { Latitude = 33.535339f, Longitude = -94.792232f },
            new Co_Ordinates { Latitude = 32.234235f, Longitude = -100.222222f }
            };

            var task = new List<Task>();
            foreach (var targetCoordinate in targetCoordinates)
            {
                task.Add(Task.Run(() => quadTree.FindNearest(targetCoordinate))
                    .ContinueWith(x => DisplayResult.Display(targetCoordinate,x.Result.VehicleId)));
            }
            Task.WaitAll(task.ToArray());
            stopwatch.Stop();
            Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds}ms");




            //foreach (var targetCoordinate in targetCoordinates)
            //{
            //    var nearestVehicle = quadTree.FindNearest(targetCoordinate.Latitude, targetCoordinate.Longitude);



            //    Console.WriteLine();
            //}

        }
    }
}
