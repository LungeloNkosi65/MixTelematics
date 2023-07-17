using MixTelematics.Models;
using System.Text;

namespace MixTelematics.Services
{
    public class BinaryFIleREader
    {

        public static List<VehiclePosition> ReadBinaryDataFile(string fileName)
        {
            string strPath = System.IO.Directory.GetCurrentDirectory();

            string filePath = Path.Combine(strPath, fileName);

            List<VehiclePosition> positions = new List<VehiclePosition>();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
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


        public static List<VehiclePosition> ReadBinaryDataFile2(string fileName)
        {
            string strPath = System.IO.Directory.GetCurrentDirectory();
            string filePath = Path.Combine(strPath, fileName);
            List<VehiclePosition> positions = new List<VehiclePosition>(1000); // Set an initial capacity

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using BinaryReader binaryReader = new BinaryReader(fileStream, Encoding.Default, true);
                byte[] buffer = new byte[4096]; // Set a larger buffer size

                while (fileStream.Position < fileStream.Length)
                {
                    int bytesRead = binaryReader.Read(buffer, 0, buffer.Length);
                    int bytesPerPosition = sizeof(int) + sizeof(float) * 2 + sizeof(ulong);
                    int positionCount = bytesRead / bytesPerPosition;

                    for (int i = 0; i < positionCount; i++)
                    {
                        int offset = i * bytesPerPosition;

                        VehiclePosition position = new VehiclePosition();

                        // Assuming VehicleId is of type int
                        position.VehicleId = BitConverter.ToInt32(buffer, offset);

                        // Assuming Latitude and Longitude are of type float
                        position.Latitude = BitConverter.ToSingle(buffer, offset + sizeof(int));
                        position.Longitude = BitConverter.ToSingle(buffer, offset + sizeof(int) + sizeof(float));

                        // Assuming RecordedTimeUTC is of type ulong
                        position.RecordedTimeUTC = BitConverter.ToUInt64(buffer, offset + sizeof(int) + sizeof(float) * 2);

                        // Assuming VehicleRegistration is a null-terminated string
                        int registrationOffset = offset + sizeof(int) + sizeof(float) * 2 + sizeof(ulong);
                        int registrationLength = FindNullTerminator(buffer, registrationOffset);
                        position.VehicleRegistration = Encoding.Default.GetString(buffer, registrationOffset, registrationLength);

                        positions.Add(position);
                    }
                }
            }

            return positions;
        }

        private static int FindNullTerminator(byte[] buffer, int startIndex)
        {
            int endIndex = Array.IndexOf<byte>(buffer, 0, startIndex);
            if (endIndex == -1)
            {
                endIndex = buffer.Length;
            }
            return endIndex - startIndex;
        }





        public static List<VehiclePosition> ReadBinaryDataFile3(string fileName)
        {
            string strPath = System.IO.Directory.GetCurrentDirectory();
            string filePath = Path.Combine(strPath, fileName);
            List<VehiclePosition> positions = new List<VehiclePosition>();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using BinaryReader binaryReader = new BinaryReader(fileStream);
                long fileSize = binaryReader.BaseStream.Length;
                int positionSizeInBytes = sizeof(int) + sizeof(float) * 2 + sizeof(ulong);
                int bufferSize = 4096;

                // Calculate the number of positions to read in parallel
                int parallelTasks = (int)Math.Ceiling((double)fileSize / bufferSize);

                Parallel.For(0, parallelTasks, taskIndex =>
                {
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;

                    lock (binaryReader)
                    {
                        binaryReader.BaseStream.Seek(taskIndex * bufferSize, SeekOrigin.Begin);
                        bytesRead = binaryReader.Read(buffer, 0, bufferSize);
                    }

                    for (int i = 0; i < bytesRead / positionSizeInBytes; i++)
                    {
                        int offset = i * positionSizeInBytes;

                        VehiclePosition position = new VehiclePosition
                        {
                            VehicleId = BitConverter.ToInt32(buffer, offset),
                            VehicleRegistration = ReadCString(buffer, offset + sizeof(int)),
                            Latitude = BitConverter.ToSingle(buffer, offset + sizeof(int) + VehicleRegistrationSize(buffer, offset + sizeof(int))),
                            Longitude = BitConverter.ToSingle(buffer, offset + sizeof(int) + VehicleRegistrationSize(buffer, offset + sizeof(int)) + sizeof(float)),
                            RecordedTimeUTC = BitConverter.ToUInt64(buffer, offset + sizeof(int) + VehicleRegistrationSize(buffer, offset + sizeof(int)) + sizeof(float) * 2)
                        };

                        lock (positions)
                        {
                            positions.Add(position);
                        }
                    }
                });
            }

            return positions;
        }

        private static string ReadCString(byte[] buffer, int startIndex)
        {
            int endIndex = Array.IndexOf<byte>(buffer, 0, startIndex);
            if (endIndex == -1)
            {
                endIndex = buffer.Length;
            }
            return Encoding.Default.GetString(buffer, startIndex, endIndex - startIndex);
        }

        private static int VehicleRegistrationSize(byte[] buffer, int startIndex)
        {
            int endIndex = Array.IndexOf<byte>(buffer, 0, startIndex);
            if (endIndex == -1)
            {
                endIndex = buffer.Length;
            }
            return endIndex - startIndex;
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

