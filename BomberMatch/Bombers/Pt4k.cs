using Position = BomberMatch.Bombers.Pt4k.Position;
using Vector = BomberMatch.Bombers.Pt4k.Vector;
using Size = BomberMatch.Bombers.Pt4k.Size;


namespace BomberMatch.Bombers
{
    /// <summary>
    /// On prns! 
    /// </summary>
    public class Pt4k : IBomber
    {
        public string Name => "Pt4k";

        private int detonationRadius;

        /// <summary>
        /// Pt4k ia prns! 
        /// </summary>
        /// <param name="matchActionsNumber"></param>
        /// <param name="detonationRadius"></param>
        /// <param name="timeToDetonate"></param>
        public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
        {
            this.detonationRadius = detonationRadius;
        }

        /// <summary>
        /// Pt4k ty prns? 
        /// </summary>
        /// <param name="arena"></param>
        /// <param name="bombers"></param>
        /// <param name="availableMoves"></param>
        /// <returns></returns>
        public int Go(int[,] arena, int[,] bombers, int[] availableMoves)
        {
            var myPosition = bombers.GetMyPosition();
            var map = arena.MapAnxiety(detonationRadius).WeightAnxiety(myPosition);

            //DEBUG!!!!!!
            //map.Draw();

            var anxiety = map[myPosition.Row, myPosition.Column];
            var next = Vector.Zero;
            var vectors = availableMoves.GetAvailableVectors();
            foreach (var vector in vectors)
            {
                var positions = map.GetDirection(myPosition, vector);
                foreach (var position in positions)
                {
                    if (map[position.Row, position.Column] < anxiety)
                    {
                        anxiety = map[position.Row, position.Column];
                        next = vector;
                    }

                    break;
                }
            }

            var result = 0;

            if (next == Vector.Zero)
                result = 0;

            if (next == Vector.Top)
                result = 1;

            if (next == Vector.Bottom)
                result = 2;

            if (next == Vector.Left)
                result = 3;

            if (next == Vector.Right)
                result = 4;

            vectors = availableMoves.GetAvailableVectors();
            foreach (var vector in vectors)
            {
                var positions = map.GetDirection(myPosition, vector);
                foreach (var position in positions)
                {
                    if (bombers.IsBomber(position) && myPosition.GetDistance(position) <= 2)
                    {
                        anxiety = 0;
                        var opposite = myPosition.GetOppositeVector(position);
                        var move = 0;

                        if (opposite == Vector.Zero)
                        {
                            anxiety = map[myPosition.Row, myPosition.Column];
                            move = 0;
                        }

                        if (opposite == Vector.Top)
                        {
                            move = 1;
                            if (availableMoves.Contains(move))
                            {
                                anxiety = map[myPosition.Row + Vector.Top.Vr, myPosition.Column + Vector.Top.Vc];
                            }
                        }

                        if (opposite == Vector.Bottom)
                        {
                            move = 2;
                            if (availableMoves.Contains(move))
                            {
                                anxiety = map[myPosition.Row + Vector.Bottom.Vr, myPosition.Column + Vector.Bottom.Vc];
                            }
                        }

                        if (opposite == Vector.Left)
                        {
                            move = 3;
                            if (availableMoves.Contains(move))
                            {
                                anxiety = map[myPosition.Row + Vector.Left.Vr, myPosition.Column + Vector.Left.Vc];
                            }
                        }

                        if (opposite == Vector.Right)
                        {
                            move = 4;
                            if (availableMoves.Contains(move))
                            {
                                anxiety = map[myPosition.Row + Vector.Right.Vr, myPosition.Column + Vector.Right.Vc];
                            }
                        }

                        if (availableMoves.Contains(move) && anxiety < 100)
                        {
                            return move + 10;
                        }
                    }
                }
            }

            var bomber = new Position(bombers[1, 0], bombers[1, 1]);
            //WAAAAAAGHH!!!!111111
            if (Pt4kExtensions.Bloodlust())
            {
                var path = arena.FindShortestPath(myPosition, bomber);
                if (path.Count > 1)
                {
                    var to = path[1];
                    if (myPosition.GetDistance(bomber) > 1)
                    {
                        var towards = myPosition.GetTowardsVector(to);
                        anxiety = map[to.Row, to.Column];
                        if (anxiety < 100)
                        {
                            if (towards == Vector.Left)
                            {
                                if (vectors.Contains(Vector.Left))
                                {
                                    return 3;
                                }
                            }

                            if (towards == Vector.Top)
                            {
                                if (vectors.Contains(Vector.Top))
                                {
                                    return 1;
                                }
                            }

                            if (towards == Vector.Right)
                            {
                                if (vectors.Contains(Vector.Right))
                                {
                                    return 4;
                                }
                            }

                            if (towards == Vector.Bottom)
                            {
                                if (vectors.Contains(Vector.Bottom))
                                {
                                    return 2;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        #region Structs

        public struct Position
        {
            public static Position None = new Position(-1, -1);

            public int Row { get; }

            public int Column { get; }

            public Position(int row, int column)
            {
                Row = row;
                Column = column;
            }
        }

        public struct Size
        {
            public int Rows { get; }

            public int Columns { get; }

            public Size(int rows, int columns)
            {
                Rows = rows;
                Columns = columns;
            }
        }

        public struct Vector
        {
            public static Vector Zero = new Vector(0, 0);

            public static Vector Left = new Vector(0, -1);

            public static Vector Top = new Vector(-1, 0);

            public static Vector Right = new Vector(0, 1);

            public static Vector Bottom = new Vector(1, 0);

            public static Vector TopLeft = new Vector(-1, -1);

            public static Vector TopRight = new Vector(-1, 1);

            public static Vector BottomLeft = new Vector(1, -1);

            public static Vector BottomRight = new Vector(1, 1);

            public static Vector[] AllDirections = new Vector[] { Left, Top, Right, Bottom };

            public int Vr { get; }

            public int Vc { get; }

            public Vector(int vr, int vc)
            {
                Vr = vr;
                Vc = vc;
            }

            public static bool operator ==(Vector v1, Vector v2)
            {
                return v1.Vr == v2.Vr && v1.Vc == v2.Vc;
            }

            public static bool operator !=(Vector v1, Vector v2)
            {
                return !(v1 == v2);
            }

            public bool Equals(Vector other)
            {
                return Vr == other.Vr && Vc == other.Vc;
            }

            public override bool Equals(object? obj)
            {
                return obj is Vector other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Vr, Vc);
            }
        }

        #endregion
    }

    /// <summary>
    /// Moar!
    /// </summary>
    internal static class Pt4kExtensions
    {
        public static bool IsWall(this int[,] area, Pt4k.Position position)
        {
            return area[position.Row, position.Column] == -1;
        }

        public static bool IsCorner(this int[,] area, Pt4k.Position position)
        {
            var size = area.GetSize();

            if (position.Row == 0 && position.Column == 0)
                return true;

            if (position.Row == 0 && position.Column == size.Rows - 1)
                return true;

            if (position.Row == size.Columns - 1 && position.Column == 0)
                return true;

            if (position.Row == size.Columns - 1 && position.Column == size.Rows - 1)
                return true;

            return false;
        }

        public static bool IsEdge(this int[,] area, Pt4k.Position position)
        {
            var size = area.GetSize();

            if (position.Row == 0 || position.Column == 0)
                return true;

            if (position.Row == size.Columns - 1 || position.Column == size.Rows - 1)
                return true;

            return false;
        }

        public static bool IsBomb(this int[,] area, Pt4k.Position position, out int time)
        {
            if (area[position.Row, position.Column] > 0)
            {
                time = area[position.Row, position.Column];
                return true;
            }

            time = 0;
            return false;
        }

        public static bool IsBomber(this int[,] bombers, Pt4k.Position position)
        {
            var count = bombers.GetUpperBound(0) + 1;
            for (var i = 0; i < count; i++)
            {
                if (bombers[i, 0] == position.Row && bombers[i, 1] == position.Column)
                    return true;
            }

            return false;
        }

        public static Vector GetOppositeVector(this Pt4k.Position p1, Pt4k.Position p2)
        {
            var vr = p1.Row - p2.Row;
            var vc = p1.Column - p2.Column;

            if (vr > 0)
                vr = 1;
            if (vr < 0)
                vr = -1;
            if (vc > 0)
                vc = 1;
            if (vc < 0)
                vc = -1;

            return new Vector(vr, vc);
        }

        public static Vector GetTowardsVector(this Pt4k.Position p1, Pt4k.Position p2)
        {
            var vr = p2.Row - p1.Row;
            var vc = p2.Column - p1.Column;

            if (vr > 0) vr = 1;
            if (vr < 0) vr = -1;
            if (vc > 0) vc = 1;
            if (vc < 0) vc = -1;

            return new Vector(vr, vc);
        }

        public static int GetDistance(this Pt4k.Position p1, Pt4k.Position p2)
        {
            return Math.Abs(p1.Row - p2.Row) + Math.Abs(p1.Column - p2.Column);
        }

        public static Size GetSize(this int[,] area)
        {
            var rows = area.GetLength(0);
            var columns = area.GetLength(1);

            return new Size(rows, columns);
        }

        public static Pt4k.Position[] GetDirection(this int[,] area, Pt4k.Position position, Vector vector)
        {
            var size = area.GetSize();

            IEnumerable<Pt4k.Position> GetDirectionImpl()
            {
                int ar = position.Row, ac = position.Column;
                while (ac >= 0 && ac < size.Columns && ar >= 0 && ar < size.Rows)
                {
                    ar += vector.Vr;
                    ac += vector.Vc;

                    if (ac < 0 || ar < 0 || ac >= size.Columns || ar >= size.Rows)
                        continue;

                    yield return new Pt4k.Position(ar, ac);
                }
            }

            return GetDirectionImpl().ToArray();
        }

        public static Vector[] GetAvailableVectors(this int[] availableMoves)
        {
            return GetAvailableImpl().ToArray();

            IEnumerable<Vector> GetAvailableImpl()
            {
                foreach (var move in availableMoves)
                {
                    switch (move)
                    {
                        case 1:
                            yield return Vector.Top;
                            break;
                        case 2:
                            yield return Vector.Bottom;
                            break;
                        case 3:
                            yield return Vector.Left;
                            break;
                        case 4:
                            yield return Vector.Right;
                            break;
                    }
                }
            }
        }

        public static Pt4k.Position GetMyPosition(this int[,] bombers)
        {
            return new Pt4k.Position(bombers[0, 0], bombers[0, 1]);
        }

        public static int[,] WeightAnxiety(this int[,] area, Pt4k.Position position)
        {
            var size = area.GetSize();
            var map = new int[size.Columns, size.Rows];

            for (var x = 0; x < size.Rows; x++)
            {
                for (var y = 0; y < size.Columns; y++)
                {
                    var distance = new Pt4k.Position(x, y).GetDistance(position);
                    if (distance > 0)
                    {
                        map[x, y] = (int)((area[x, y] / (distance * 2)));
                    }
                    else
                    {
                        map[x, y] = area[x, y];
                    }
                }
            }

            return map;
        }

        public static int[,] MapAnxiety(this int[,] arena, int detonationRadius)
        {
            var size = arena.GetSize();
            var map = new int[size.Columns, size.Rows];

            for (var x = 0; x < size.Rows; x++)
            {
                for (var y = 0; y < size.Columns; y++)
                {
                    var position = new Pt4k.Position(x, y);

                    if (arena.IsWall(position))
                    {
                        map[position.Row, position.Column] -= 25;
                    }

                    if (arena.IsCorner(position))
                    {
                        map[position.Row, position.Column] += 50;
                    }

                    if (arena.IsEdge(position))
                    {
                        map[position.Row, position.Column] += 25;
                    }

                    if (arena.IsBomb(position, out var time))
                    {
                        var explosiveness = 500;

                        if (time > 0)
                        {
                            explosiveness += (int)(1 / time * 100d);
                        }

                        if (time == 1)
                            explosiveness += 1000;

                        map[position.Row, position.Column] += explosiveness;
                        foreach (var vector in Vector.AllDirections)
                        {
                            var direction = arena.GetDirection(position, vector);
                            var count = 0;
                            foreach (var next in direction)
                            {
                                if (arena.IsWall(next))
                                    break;

                                if (count >= detonationRadius)
                                    break;

                                map[next.Row, next.Column] += explosiveness;
                                count++;
                            }
                        }
                    }
                }
            }

            return map;
        }

        public static bool Bloodlust()
        {
            var rnd = new Random();
            var random = rnd.Next(1, 6);

            return random != 3;
        }

        public static List<Pt4k.Position> FindShortestPath(this int[,] array, Pt4k.Position start, Pt4k.Position end)
        {
            var size = array.GetSize();
            var rows = size.Rows;
            var cols = size.Columns;

            bool[,] visited = new bool[rows, cols];
            Dictionary<Pt4k.Position, Pt4k.Position> previousPositions = new Dictionary<Pt4k.Position, Pt4k.Position>();
            Queue<Pt4k.Position> queue = new Queue<Pt4k.Position>();
            queue.Enqueue(start);
            visited[start.Row, start.Column] = true;
            while (queue.Count > 0)
            {
                Pt4k.Position current = queue.Dequeue();
                if (current.Row == end.Row && current.Column == end.Column)
                    break;

                TraverseNeighbors(array, visited, previousPositions, queue, current, rows, cols);
            }

            List<Pt4k.Position> shortestPath = new List<Pt4k.Position>();
            Pt4k.Position position = end;
            while (!Equals(position, Pt4k.Position.None))
            {
                shortestPath.Insert(0, position);
                position = previousPositions.ContainsKey(position) ? previousPositions[position] : Pt4k.Position.None;
            }
            return shortestPath;
        }
        static void TraverseNeighbors(int[,] array, bool[,] visited, Dictionary<Pt4k.Position, Pt4k.Position> previousPositions, Queue<Pt4k.Position> queue, Pt4k.Position current, int rows, int cols)
        {
            int[] rowOffsets = { -1, 1, 0, 0 };
            int[] colOffsets = { 0, 0, -1, 1 };
            for (int i = 0; i < 4; i++)
            {
                int newRow = current.Row + rowOffsets[i];
                int newCol = current.Column + colOffsets[i];
                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols &&
                !visited[newRow, newCol])
                {
                    if (array[newRow, newCol] == 0)
                    {
                        Pt4k.Position newPosition = new Pt4k.Position(newRow, newCol);
                        visited[newRow, newCol] = true;
                        queue.Enqueue(newPosition);
                        previousPositions[newPosition] = current;
                    }
                }
            }
        }

        public static void Draw(this int[,] area)
        {
            var size = area.GetSize();

            Console.WriteLine();
            for (var x = 0; x < size.Rows; x++)
            {
                for (var y = 0; y < size.Columns; y++)
                {
                    Console.Write(area[x, y].ToString(" +000; -000"));
                }

                Console.WriteLine();
            }
        }
    }
}
