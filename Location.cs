using System;
using System.Numerics;

namespace CombSim
{
    public class Location : IEquatable<Location>
    {
        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; }
        public int y { get; }

        public bool Equals(Location obj)
        {
            return this == obj;
        }

        public override bool Equals(object o)
        {
            if (o is Location)
            {
                return this == (Location)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 13 * x.GetHashCode() + y.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }

        public float DistanceBetween(Location other)
        {
            var distance = (float)Math.Sqrt(Math.Pow(x - other.x, 2f) + Math.Pow(y - other.y, 2f));
            return distance;
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

        public static Location operator +(Location lhs, Vector2 rhs)
        {
            return new Location(lhs.x + (int)Math.Round(rhs.X), lhs.y + (int)Math.Round(rhs.Y));
        }
    }
}