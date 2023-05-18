using System;
using System.Numerics;

namespace CombSim
{
    public class Location : IEquatable<Location>
    {
        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; }
        public int Y { get; }

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
            return 13 * X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        public float DistanceBetween(Location other)
        {
            var distance = (float)Math.Sqrt(Math.Pow(X - other.X, 2f) + Math.Pow(Y - other.Y, 2f));
            return distance;
        }

        public static bool operator ==(Location lhs, Location rhs)
        {
            if (lhs is null && rhs is null) return true;
            if (lhs is null || rhs is null) return false;
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        public static bool operator !=(Location lhs, Location rhs)
        {
            return !(lhs == rhs);
        }

        public static Location operator +(Location lhs, Vector2 rhs)
        {
            return new Location(lhs.X + (int)Math.Round(rhs.X), lhs.Y + (int)Math.Round(rhs.Y));
        }
    }
}