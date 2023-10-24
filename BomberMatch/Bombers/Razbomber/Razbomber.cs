using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMatch.Bombers.Razbomber
{
    /*
   ___                    _                       _                      
  | _ \   __ _      ___  | |__     ___    _ __   | |__     ___      _ _  
  |   /  / _` |    |_ /  | '_ \   / _ \  | '  \  | '_ \   / -_)    | '_| 
  |_|_\  \__,_|   _/__|  |_.__/   \___/  |_|_|_| |_.__/   \___|   _|_|_  
_|"""""|_|"""""|_|"""""|_|"""""|_|"""""|_|"""""|_|"""""|_|"""""|_|"""""| 
"`-0-0-'"`-0-0-'"`-0-0-'"`-0-0-'"`-0-0-'"`-0-0-'"`-0-0-'"`-0-0-'"`-0-0-' 
     */

    public class Razbomber : IBomber
    {
        private const int GoTop = 1;
        private const int GoBottom = 2;
        private const int GoLeft = 3;
        private const int GoRight = 4;
        private const int Plant = 10;

        public string Name => "Razbomber";

        private int detonationRadius;
        private int timeToDetonate;

        public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
        {
            this.detonationRadius = detonationRadius;
            this.timeToDetonate = timeToDetonate - 1; //на самом деле на 1 ход меньше
        }

        public int Go(int[,] arena, int[,] bombers, int[] availableMoves)
        {
            arena = EditDetonationDelaysByCascades(arena);

            var self = new Point(bombers[0, 0], bombers[0, 1]);
            var enemies = GetEnemiesPositions(bombers);

            //получение соседних клеток, при попадании в которые бомбер не обрекает себя на подрыв
            var neighborsOutsideDeadzones = GetNeighborsOutsideDeadzones(arena, self);
            if (neighborsOutsideDeadzones.Count == 0)
                return Plant;

            int optimalDistance = Math.Max(1, timeToDetonate - detonationRadius);
            var pointToMove = GetClosestToOptimalDistanceToEnemy(
                arena,
                neighborsOutsideDeadzones,
                self,
                enemies,
                optimalDistance
            );

            var closest = arena.GetClosest(pointToMove, (_, point) => enemies.Any(e => e.Row == point.Row && e.Column == point.Column));

            var realTimeToDetonate = GetTimeToDetonate(arena, pointToMove);
            var recalcOptimalDistance = Math.Max(1, realTimeToDetonate - detonationRadius);
            int action = 0;
            if (closest.Item2 <= recalcOptimalDistance)
            {
                action += Plant;
            }

            return action + FindDirection(self, pointToMove);
        }

        #region Decisions

        private List<Point> GetNeighborsOutsideDeadzones(int[,] arena, Point self)
        {
            var neighbors = arena.GetNeighbors(self);
            return neighbors.Where(neighbor =>
            {
                var (bombLocation, bombDistance) = arena.GetClosest(neighbor.Item2, (value, _) => value > 0);
                if (bombLocation == default) //нет бомб
                    return true;

                int deadZoneRadius = (detonationRadius - arena[bombLocation.Row, bombLocation.Column]) + 1;
                if (bombDistance > deadZoneRadius)
                    return true;

                return false;
            }).Select(pair => pair.Item2).ToList();
        }

        private Point GetClosestToOptimalDistanceToEnemy(int[,] arena, List<Point> availableMoves, Point self, List<Point> enemies, int optimalDistance)
        {
            return availableMoves.MinBy(move =>
            {
                var closest = arena.GetClosest(move, (_, point) => enemies.Any(e => e.Row == point.Row && e.Column == point.Column));
                return Math.Abs(closest.Item2 - optimalDistance);
            });
        }

        private int FindDirection(Point current, Point pointToMove)
        {
            if (current.Row - 1 == pointToMove.Row)
                return GoTop;

            if (current.Row + 1 == pointToMove.Row)
                return GoBottom;

            if (current.Column - 1 == pointToMove.Column)
                return GoLeft;

            if (current.Column + 1 == pointToMove.Column)
                return GoRight;

            return 0;
        }

        #endregion

        #region Cascades

        /// <summary>
        /// Копирует карту и заменяет время до взрыва бомб, если взрыв будет вызван взрывом соседей
        /// </summary>
        private int[,] EditDetonationDelaysByCascades(int[,] arena)
        {
            var copy = new int[arena.GetLength(0), arena.GetLength(1)];
            arena.ForEachCell((item, row, col) => copy[row, col] = item);

            var bombsLocations = new List<Point>();
            copy.ForEachCell((item, row, col) =>
            {
                if (item > 0)
                    bombsLocations.Add(new Point(row, col));
            });

            bool changed;
            do
            {
                changed = false;
                foreach (var location in bombsLocations)
                {
                    var (closestPoint, distance) = arena.GetClosest(location, (item, point) => !Point.Same(location, point) && item > 0);
                    if (closestPoint == default || distance > this.detonationRadius)
                        continue;

                    var current = copy[location.Row, location.Column];
                    var closest = copy[closestPoint.Row, closestPoint.Column];
                    if (current == closest)
                        continue;
                    //если две бомбы в пределах радиуса взрыва, то обе взорвутся тогда, когда взорвется первая из них
                    copy[location.Row, location.Column] = copy[closestPoint.Row, closestPoint.Column] = Math.Min(current, closest);
                    changed = true;
                }
            } while (changed); //бегать пока не утрясется. Made fast, works slow.

            return copy;
        }

        /// <summary>
        /// Получает время до взрыва с учетом возможного каскадного подрыва
        /// </summary>
        private int GetTimeToDetonate(int[,] arena, Point current)
        {
            var (closestPoint, distance) = arena.GetClosest(current, (item, point) => !Point.Same(point, current) && item > 0);
            if (closestPoint == default || distance > this.detonationRadius) //нет бомб или слишком далеко
                return this.timeToDetonate;

            //а что а вдруг
            var min = Math.Min(this.timeToDetonate, arena[closestPoint.Row, closestPoint.Column]);
            return min - 1; //на самом деле на 1 ход меньше
        }

        #endregion

        private List<Point> GetEnemiesPositions(int[,] bombers)
        {
            var result = new List<Point>();
            for (int i = 1; i < bombers.GetLength(0); i++)
            {
                result.Add(new Point(bombers[i, 0], bombers[i, 1]));
            }
            return result;
        }
    }
}
