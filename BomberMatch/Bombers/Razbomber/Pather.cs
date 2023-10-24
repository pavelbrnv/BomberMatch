using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMatch.Bombers.Razbomber
{
    internal static class Pather
    {

        #region FindDistance

        /// <summary>
        /// Нахождение минимального расстояния между двумя точками по алгоритму Дейкстры
        /// </summary>
        /// <param name="start"></param>
        /// <param name="destination"></param>
        /// <param name="arena"></param>
        /// <returns></returns>
        internal static int FindDistance(Point start, Point destination, int[,] arena)
        {
            var map = GetDijkstraMap(arena, start);

            while (true)
            {
                var pointToProcess = FindPointToProcess(map);
                if (pointToProcess == null) //все точки посещены
                    break;

                var itemToProcess = map[pointToProcess.Row, pointToProcess.Column];
                int newDistance = itemToProcess.Distance + 1;
                map.GetNeighbors(pointToProcess).ForEach(pair => 
                {
                    var (neighbor, _) = pair;
                    if (neighbor.Reachable)
                        neighbor.Distance = Math.Min(neighbor.Distance, newDistance);
                });

                itemToProcess.Visited = true;
            }

            return map[destination.Row, destination.Column].Distance;
        }

        private static DijkstraPoint[,] GetDijkstraMap(int[,] arena, Point startPoint) 
        { 
            var result = new DijkstraPoint[arena.GetLength(0), arena.GetLength(1)];
            arena.ForEachCell((item, row, column) =>
            {
                result[row, column] = new DijkstraPoint(
                    int.MaxValue,
                    arena[row, column]
                );
            });

            result[startPoint.Row, startPoint.Column].Distance = 0;
            return result;
        }

        private static Point FindPointToProcess(DijkstraPoint[,] map)
        {
            var minDistance = int.MaxValue;
            Point point = null;

            map.ForEachCell((current, row, column) =>
            {
                if (!current.Visited && current.Reachable && current.Distance < minDistance)
                {
                    minDistance = current.Distance;
                    point = new Point(row, column);
                }
            });

            return point;
        }

        #endregion

        #region FindNeighbors

        public static List<Point> FindNeighborsInRange(int[,] arena, Point start, int range)
        {
            var set = new HashSet<int>();
            FindNeighborsInRangeInternal(arena, start, range, set);

            return set.Select(address => arena.Deserialize(address)).ToList();
        }

        private static void FindNeighborsInRangeInternal(int[,] arena, Point start, int range, HashSet<int> result)
        {
            if (range < 0) return;
            int columnCount = arena.GetLength(1);

            var startAddress = start.Serialize(columnCount);
            if (!result.Add(startAddress)) return; //если такая точка уже добавлена

            arena.GetNeighbors(start).ForEach(pair =>
            {
                var (_, neighbor) = pair;
                FindNeighborsInRangeInternal(arena, neighbor, range - 1, result);
            });
        }

        #endregion
    }
}
