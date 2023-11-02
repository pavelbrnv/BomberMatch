using System.Runtime.Serialization.Formatters;

namespace BomberMatch.Bombers
{

    public enum BomberState
    {
        FIND,
        MOVE,
        KILL,
        ESCAPE,
    }

    public class Rabkahalla : IBomber
    {
        private int detonationRadius;
        private int timeToDetonate;
        private int maxActionsNumber;

        private BomberState currentState = BomberState.FIND;
        private BomberState newState = BomberState.FIND;
        public string Name => "Rabkahalla";

        public int Go(int[,] arena, int[,] bombers, int[] availableActions)
        {
            Position self = new Position(bombers[0, 1], bombers[0, 0]);
            var enemies = GetEnemies(bombers);
            int result = 0;

            var target = ClosestTarget(arena, enemies, self);
            if (target.Item2.Count == 1 || target.Item2.Count == 2)
            {
                result += 10;
                result += GetSafeDirection(arena, self, availableActions);
            }
            else if (target.Item2.Count > 2)
            {
                var direction = GetDirection(self, target.Item2[1]);
                if (availableActions.Contains(direction))
                {
                    result += direction;
                }
                else
                {
                    result += GetSafeDirection(arena, self, availableActions);
                }
            }
            else
            {
                result += GetSafeDirection(arena, self, availableActions);
            }

            return result;
        }

        public void SetRules(int maxActionsNumber, int detonationRadius, int timeToDetonate)
        {

            this.detonationRadius = detonationRadius;
            this.timeToDetonate = timeToDetonate;
            this.maxActionsNumber = maxActionsNumber;
        }

        private int GetSafeDirection(int[,] arena, Position self, int[] availableActions)
        {
            int radius = detonationRadius;
            while (radius > 0)
            {
                for (int i = 0; i < availableActions.Length; i++)
                {
                    var newPosition = GetPosition(self, availableActions[i]);
                    if (IsDangerous(arena, newPosition, radius))
                    {
                        continue;
                    }
                    else
                    {
                        return GetDirection(self, newPosition);
                    }
                }
                radius--;
            }
            return 0;
        }


        private List<Position> GetEnemies(int[,] bombers)
        {
            List<Position> result = new List<Position>();
            for (int i = 1; i < bombers.GetLength(0); i++)
            {
                var bomber = new Position(bombers[i, 1], bombers[i, 0]);
                result.Add(bomber);
            }
            return result;
        }

        private Position GetPosition(Position self, int action)
        {
            switch (action)
            {
                case 1: return new Position(self.X, self.Y - 1);
                case 2: return new Position(self.X, self.Y + 1);
                case 3: return new Position(self.X - 1, self.Y);
                case 4: return new Position(self.X + 1, self.Y);
                default: return self;
            }
        }

        private int GetDirection(Position self, Position target)
        {
            if (self.X > target.X)
            {
                return 3;
            }
            else if (self.Y > target.Y)
            {
                return 1;
            }
            else if (self.X < target.X)
            {
                return 4;
            }
            else if (self.Y < target.Y)
            {
                return 2;
            }
            else { return 0; }
        }
        private int ReverseDirection(int direction)
        {
            return direction switch
            {
                0 => 0,
                1 => 2,
                2 => 1,
                3 => 4,
                4 => 3,
                _ => 0,
            };
        }

        private Tuple<Position, List<Position>> ClosestTarget(int[,] arena, List<Position> enemies, Position self)
        {
            List<Position> targetPath = new List<Position>();
            Position target = new Position(0, 0);
            foreach (var position in enemies)
            {
                var path = FindPath(arena, self, position);
                if (targetPath.Count == 0 || targetPath.Count > path.Count)
                {
                    targetPath = path;
                    target = position;
                }
            }
            return new Tuple<Position, List<Position>>(target, targetPath);
        }


        private List<Position> FindPath(int[,] arena, Position start, Position goal)
        {
            var closedSet = new List<PathNode>();
            var openSet = new List<PathNode>();

            PathNode startNode = new PathNode()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                DistanceToTarget = GetDistanceToTarget(start, goal)
            };
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(node => node.FullPathLength).First();

                if (currentNode.Position == goal)
                {
                    return GetPathForNode(currentNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbourNode in GetNeighbours(currentNode, goal, arena))
                {

                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                        continue;
                    var openNode = openSet.FirstOrDefault(node => node.Position == neighbourNode.Position);

                    if (openNode == null)
                    {
                        openSet.Add(neighbourNode);
                    }
                    else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }
            return new List<Position> { };
        }

        private static int GetDistanceToTarget(Position from, Position to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        private static List<Position> GetPathForNode(PathNode pathNode)
        {
            var result = new List<Position>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }

        private List<PathNode> GetNeighbours(PathNode pathNode, Position goal, int[,] area)
        {
            var result = new List<PathNode>();

            Position[] neighbourPoints = GetNearPositions(pathNode);

            foreach (var point in neighbourPoints)
            {

                if (point.X < 0 || point.X >= area.GetLength(0))
                    continue;
                if (point.Y < 0 || point.Y >= area.GetLength(1))
                    continue;

                if ((area[point.X, point.Y] != 0) || IsDangerous(area, point, detonationRadius))
                    continue;

                var neighbourNode = new PathNode()
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart + 1,
                    DistanceToTarget = GetDistanceToTarget(point, goal)
                };
                result.Add(neighbourNode);
            }
            return result;
        }

        private bool IsDangerous(int[,] area, Position checkNode, int radius)
        {

            Position[] neighbourPoints = new Position[radius * 4];
            for (int i = 0; i < neighbourPoints.Length; i = i + 4)
            {
                neighbourPoints[i] = new Position(checkNode.X + i / 4 + 1, checkNode.Y);
                neighbourPoints[i + 1] = new Position(checkNode.X - i / 4 - 1, checkNode.Y);
                neighbourPoints[i + 2] = new Position(checkNode.X, checkNode.Y + i / 4 + 1);
                neighbourPoints[i + 3] = new Position(checkNode.X, checkNode.Y - i / 4 - 1);
            }
            foreach (var point in neighbourPoints)
            {
                if (point.X < 0 || point.X >= area.GetLength(0))
                    continue;
                if (point.Y < 0 || point.Y >= area.GetLength(1))
                    continue;
                if ((area[point.Y, point.X] > 0))
                    return true;
            }

            if (IsDeadEnd(area, checkNode))
            {
                return true;
            }

            return false;
        }

        private bool IsDeadEnd(int[,] area, Position checkNode)
        {
            Position[] neighbourPoints = new Position[4];
            neighbourPoints[0] = new Position(checkNode.X + 1, checkNode.Y);
            neighbourPoints[1] = new Position(checkNode.X - 1, checkNode.Y);
            neighbourPoints[2] = new Position(checkNode.X, checkNode.Y + 1);
            neighbourPoints[3] = new Position(checkNode.X, checkNode.Y - 1);

            int count = 0;
            for (int i = 0; i < neighbourPoints.Length; i++)
            {
                if (neighbourPoints[i].X < 0 || neighbourPoints[i].X >= area.GetLength(0) || neighbourPoints[i].Y < 0 || neighbourPoints[i].Y >= area.GetLength(1) || area[neighbourPoints[i].Y, neighbourPoints[i].X] != 0)
                {
                    count++;
                }
            }

            if (count >= 3) return true;
            return false;



        }
        private static Position[] GetNearPositions(PathNode checkNode)
        {
            Position[] neighbourPoints = new Position[4];
            neighbourPoints[0] = new Position(checkNode.Position.X + 1, checkNode.Position.Y);
            neighbourPoints[1] = new Position(checkNode.Position.X - 1, checkNode.Position.Y);
            neighbourPoints[2] = new Position(checkNode.Position.X, checkNode.Position.Y + 1);
            neighbourPoints[3] = new Position(checkNode.Position.X, checkNode.Position.Y - 1);
            return neighbourPoints;
        }
    }

    public readonly struct Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static bool operator ==(Position first, Position second) => first.X == second.X && first.Y == second.Y;

        public static bool operator !=(Position first, Position second) => first.X != second.X || first.Y != second.Y;
    }

    public class PathNode
    {
        public Position Position { get; set; }
        public int PathLengthFromStart { get; set; }
        public PathNode? CameFrom { get; set; }
        public int DistanceToTarget { get; set; }
        public int FullPathLength => PathLengthFromStart + DistanceToTarget;
    }

}