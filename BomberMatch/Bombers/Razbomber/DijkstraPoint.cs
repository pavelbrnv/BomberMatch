using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMatch.Bombers.Razbomber
{
    internal class DijkstraPoint : IComparable<int>
    {
        public DijkstraPoint(int distance, int value)
        {
            Distance = distance;
            Value = value;
        }

        public bool Reachable => Value == 0;

        public bool Visited { get; set; }

        public int Distance { get; set; }

        public int Value { get; set; }

        public int CompareTo(int other)
        {
            if (Value == other) return 0;
            if (Value < other) return -1;

            return 1;
        }
    }
}
