using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMatch.Bombers.Razbomber
{
    internal static class Extensions
    {
        public static void ForEachCell<T>(this T[,] arena, Action<T, int, int> action)
        {
            for (int row = 0; row < arena.GetLength(0); row++)
            {
                for (int column = 0; column < arena.GetLength(1); column++)
                {
                    action(arena[row, column], row, column);
                }
            }
        }

        public static List<(T, Point)> GetNeighbors<T>(this T[,] arena, Point point)
            where T : IComparable<int>
        {
            var result = new List<(T, Point)>();

            void TryAddNeighbor(bool isInRange, int row, int column)
            {
                if (!isInRange) 
                    return;

                var current = arena[row, column];

                //if a wall or a bomb
                if (current.CompareTo(-1) == 0 || current.CompareTo(0) > 0)
                    return;
                result.Add(new (current, new Point(row, column)));
            }

            TryAddNeighbor(point.Row != 0, point.Row - 1, point.Column);
            TryAddNeighbor(point.Row != arena.GetLength(0) - 1, point.Row + 1, point.Column);

            TryAddNeighbor(point.Column != 0, point.Row, point.Column - 1);
            TryAddNeighbor(point.Column != arena.GetLength(1) - 1, point.Row, point.Column + 1);

            return result;
        }

        public static int Serialize<T>(this T[,] arena, Point p) => p.Serialize(arena.GetLength(1));
        public static Point Deserialize<T>(this T[,] arena, int address) => Point.Deserialize(address, arena.GetLength(1));

        /// <returns>(точка, расстояние)</returns>
        public static (Point, int) GetClosest<T>(this T[,] arena, Point start, Func<T, Point, bool> predicate)
            where T : IComparable<int>
        {
            return GetClosestInternal(arena, start, predicate, 0, new HashSet<int>());
        }

        private static (Point, int) GetClosestInternal<T>(T[,] arena, Point point, Func<T, Point, bool> predicate, int distance, HashSet<int> checkedPoints)
            where T : IComparable<int>
        {
            if (predicate(arena[point.Row, point.Column], point))
                return new (point, distance);

            if (!checkedPoints.Add(arena.Serialize(point))) //если уже пройден
                return default;

            var neighbors = arena.GetNeighbors(point);
            if (neighbors.Count == 0)
                return default;

            var results = neighbors.Select(neighbor => GetClosestInternal(arena, neighbor.Item2, predicate, distance + 1, checkedPoints));
            if (!results.Any())
                return default;

            return results.MinBy(pair => pair.Item1 != null ? pair.Item2 : int.MaxValue);
        }
    }
}
