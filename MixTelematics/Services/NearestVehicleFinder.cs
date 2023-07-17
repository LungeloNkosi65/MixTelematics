using MixTelematics.Models;

namespace MixTelematics.Services
{
    public class NearestVehicleFinder
    {
        private const int MaxPositionsPerNode = 10;
        private QuadTreeNode _root;


        public NearestVehicleFinder(List<VehiclePosition> positions)
        {
            _root = BuildKDTree(positions, 0, positions.Count - 1, 0);
        }
        public void Build(List<VehiclePosition> positions)
        {
            _root = BuildKDTree(positions, 0, positions.Count - 1, 0);
        }
        private QuadTreeNode BuildKDTree(List<VehiclePosition> positions, int start, int end, int depth)
        {
            if (start > end)
                return null;

            int axis = depth % 2;
            int mid = Quickselect(positions, start, end, axis, (start + end) / 2);

            var node = new QuadTreeNode
            {
                Vehicle = positions[mid],
                Left = BuildKDTree(positions, start, mid - 1, depth + 1),
                Right = BuildKDTree(positions, mid + 1, end, depth + 1)
            };

            return node;
        }
        private int Quickselect(List<VehiclePosition> positions, int start, int end, int axis, int k)
        {
            while (start < end)
            {
                int pivot = Partition(positions, start, end, axis);
                if (k < pivot)
                    end = pivot - 1;
                else if (k > pivot)
                    start = pivot + 1;
                else
                    return pivot;
            }

            return start;
        }
        private int Partition(List<VehiclePosition> positions, int start, int end, int axis)
        {
            VehiclePosition pivotValue = positions[end];
            int i = start;
            for (int j = start; j < end; j++)
            {
                if ((axis == 0 && positions[j].Longitude < pivotValue.Longitude) ||
                    (axis == 1 && positions[j].Latitude < pivotValue.Latitude))
                {
                    Swap(positions, i, j);
                    i++;
                }
            }
            Swap(positions, i, end);
            return i;
        }

        private void Swap(List<VehiclePosition> positions, int i, int j)
        {
            VehiclePosition temp = positions[i];
            positions[i] = positions[j];
            positions[j] = temp;
        }
        public VehiclePosition FindNearest(float latitude, float longitude)
        {
            if (_root == null)
                return null;

            float bestDistance = float.MaxValue;
            VehiclePosition nearest = null;

            FindNearestVehicle(_root, latitude, longitude, 0, ref bestDistance, ref nearest);

            return nearest;
        }


        private void FindNearestVehicle(QuadTreeNode node, float latitude, float longitude, int depth, ref float bestDistance, ref VehiclePosition nearest)
        {
            if (node == null)
                return;

            float distance = CalculateDistance(latitude, longitude, node.Vehicle.Latitude, node.Vehicle.Longitude);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                nearest = node.Vehicle;
            }

            int axis = depth % 2;

            if (axis == 0 ? longitude < node.Vehicle.Longitude : latitude < node.Vehicle.Latitude)
            {
                FindNearestVehicle(node.Left, latitude, longitude, depth + 1, ref bestDistance, ref nearest);
                if (axis == 0 ? (longitude + bestDistance) >= node.Vehicle.Longitude : (latitude + bestDistance) >= node.Vehicle.Latitude)
                    FindNearestVehicle(node.Right, latitude, longitude, depth + 1, ref bestDistance, ref nearest);
            }
            else
            {
                FindNearestVehicle(node.Right, latitude, longitude, depth + 1, ref bestDistance, ref nearest);
                if (axis == 0 ? (longitude - bestDistance) <= node.Vehicle.Longitude : (latitude - bestDistance) <= node.Vehicle.Latitude)
                    FindNearestVehicle(node.Left, latitude, longitude, depth + 1, ref bestDistance, ref nearest);
            }
        }

        private float CalculateDistance(float lat1, float lon1, float lat2, float lon2)
        {
            float r = 6371; // Earth's radius in kilometers

            float dLat = DegreeToRadian(lat2 - lat1);
            float dLon = DegreeToRadian(lon2 - lon1);

            float a = MathF.Sin(dLat / 2) * MathF.Sin(dLat / 2) +
                      MathF.Cos(DegreeToRadian(lat1)) * MathF.Cos(DegreeToRadian(lat2)) *
                      MathF.Sin(dLon / 2) * MathF.Sin(dLon / 2);

            float c = 2 * MathF.Atan2(MathF.Sqrt(a), MathF.Sqrt(1 - a));

            return r * c;
        }

        private float DegreeToRadian(float degree)
        {
            return degree * (MathF.PI / 180);
        }

    }
}
