using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMatch.Bombers.Razbomber
{
    internal class Point
    {
        public Point(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }

        public int Serialize(int columnCount) => Column + (Row * columnCount);

        public static Point Deserialize(int address, int columnCount)
        {
            return new Point(
                address / columnCount,
                address % columnCount
            );
        }

        public static bool Same(Point f, Point s)
        {
            return f.Row == s.Row && f.Column == s.Column;
        }
    }
}
