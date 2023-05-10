using System;
using System.Runtime.Remoting.Proxies;

namespace CombSim
{
    public class Location : IEquatable<Location>
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }

        public float DistanceBetween(Location other)
        {
            float distance = (float)Math.Sqrt(Math.Pow(x - other.x, 2f) + Math.Pow(y - other.y, 2f));
            return distance;
        }

        public bool Equals(Location obj)
        {
            return this == obj;
        }

        public static bool operator ==(Location lhs, Location rhs)
        {
            if (lhs is null && rhs is null) return true;
            if (lhs is null || rhs is null) return false;
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Location lhs, Location rhs)
        {
            return !(lhs == rhs);
        }
    }
}