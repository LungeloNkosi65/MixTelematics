using MixTelematics.Models;
using System.IO;
using System.Text;

namespace MixTelematics.Services
{
    public class BinaryFIleREader
    {

        public static List<VehiclePosition> ReadBinaryDataFile(string fileName)
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                List<VehiclePosition> positions = new List<VehiclePosition>();

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using BinaryReader binaryReader = new BinaryReader(fileStream);
                    while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                    {
                        VehiclePosition position = new VehiclePosition
                        {
                            VehicleId = binaryReader.ReadInt32(),
                            VehicleRegistration = binaryReader.ReadCString(),
                            Latitude = binaryReader.ReadSingle(),
                            Longitude = binaryReader.ReadSingle(),
                            RecordedTimeUTC = binaryReader.ReadUInt64()
                        };

                        positions.Add(position);
                    }
                }

                return positions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file. Exception: {ex.ToString()}");
                throw ex;   
            }
        }

    }

    public static class Extentions
    {
        public static string ReadCString(this BinaryReader binaryReader)
        {
            List<byte> bytes = new List<byte>();
            byte b;

            while ((b = binaryReader.ReadByte()) != 0)
            {
                bytes.Add(b);
            }

            return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
        }
    }
}

