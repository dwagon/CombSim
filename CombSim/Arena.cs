using System;
using System.Collections.Generic;
using System.Numerics;

namespace CombSim
{
    public enum GridDirection
    {
        // ReSharper disable All
        N = 0,
        NE = 45,
        E = 90,
        SE = 135,
        S = 180,
        SW = 225,
        W = 270,

        NW = 315
        // ReSharper restore All
    }

    public class Arena
    {
        private readonly object[,] _grid;
        private readonly int _maxX;
        private readonly int _maxY;

        public Arena(int maxX, int maxY)
        {
            _grid = new object[maxX, maxY];
            _maxX = maxX;
            _maxY = maxY;
            for (var x = 0; x < _maxX; x++)
            for (var y = 0; y < _maxY; y++)
                _grid[x, y] = null;
        }

        public object GetLocation(Location location)
        {
            return _grid[location.X, location.Y];
        }

        // Print the arena out
        public new string ToString()
        {
            var output = "";
            for (var y = 0; y < _maxY; y++)
            {
                for (var x = 0; x < _maxX; x++)
                    if (_grid[x, y] == null)
                    {
                        output += ".";
                    }
                    else
                    {
                        var creature = _grid[x, y] as Creature;
                        output += creature?.GetRepr();
                    }

                output += "\n";
            }

            return output;
        }


        public float DistanceBetween(Location a, Location b)
        {
            var distance = (float)Math.Sqrt(Math.Pow(a.X - b.X, 2f) + Math.Pow(a.Y - b.Y, 2f));
            return distance;
        }

        // Return an empty spot on the arena
        public bool Pick_Empty_Spot(out int x, out int y)
        {
            x = -1;
            y = -1;
            for (var attempt = 0; attempt < _maxX * _maxY; attempt++)
            {
                var rnd = new Random();
                x = rnd.Next() % _maxX;
                y = rnd.Next() % _maxY;
                if (_grid[x, y] == null) return true;
            }

            return false;
        }

        public void Set(int x, int y, object thing)
        {
            _grid[x, y] = thing;
        }

        public void Set(Location location, object thing)
        {
            var x = location.X;
            var y = location.Y;
            _grid[x, y] = thing;
        }

        public void Clear(Location location)
        {
            Set(location, null);
        }

        public bool IsClear(Location location)
        {
            return _grid[location.X, location.Y] == null;
        }

        private bool IsValidLocation(int x, int y)
        {
            return x >= 0 && x < _maxX && y >= 0 && y < _maxY;
        }

        private bool IsValidLocation(Location location)
        {
            return IsValidLocation(location.X, location.Y);
        }

        public List<Location> GetNeighbours(Location location)
        {
            var neighbours = new List<Location>();
            for (var x = Math.Max(0, location.X - 1); x <= Math.Min(_maxX - 1, location.X + 1); x++)
            for (var y = Math.Max(0, location.Y - 1); y <= Math.Min(_maxY - 1, location.Y + 1); y++)
            {
                if (x == location.X && y == location.Y) continue;
                neighbours.Add(new Location(x, y));
            }

            return neighbours;
        }

        public IEnumerable<Location> CircleLocations(Location origin, int radius)
        {
            var locations = new HashSet<Location>();

            for (var i = origin.X - radius - 1; i < origin.X + radius + 1; i++)
            {
                for (var j = origin.Y - radius - 1; j < origin.Y + radius + 1; j++)
                {
                    if (!IsValidLocation(i, j)) continue;
                    var location = new Location(i, j);
                    if (DistanceBetween(origin, location) <= radius)
                    {
                        locations.Add(location);
                    }
                }
            }

            return locations;
        }

        // Return a list of all locations from the {origin} of size {coneSize} in {direction}
        // Cone is a 60 degree section
        public IEnumerable<Location> ConeLocations(Location origin, int coneSize, GridDirection direction)
        {
            var locations = new HashSet<Location>();
            var originVector = new Vector2(origin.X, origin.Y);

            // Vertices of triangle
            var v1 = new Vector2(origin.X, origin.Y) - originVector; // Should always be <0,0>
            var v2 = new Vector2(origin.X + coneSize / 2f, origin.Y - coneSize) - originVector;
            var v3 = new Vector2(origin.X - coneSize / 2f, origin.Y - coneSize) - originVector;

            // Vertices rotated
            var v1R = RotateVector(v1, direction);
            var v2R = RotateVector(v2, direction);
            var v3R = RotateVector(v3, direction);

            // Calculate bounding box of triangle (generously)
            var minX = (int)Math.Round(Math.Min(v1R.X, Math.Min(v2R.X, v3R.X))) - 1;
            var maxX = (int)Math.Round(Math.Max(v1R.X, Math.Max(v2R.X, v3R.X))) + 1;
            var minY = (int)Math.Round(Math.Min(v1R.Y, Math.Min(v2R.Y, v3R.Y))) - 1;
            var maxY = (int)Math.Round(Math.Max(v1R.Y, Math.Max(v2R.Y, v3R.Y))) + 1;

            for (var i = minX; i < maxX; i++)
            {
                for (var j = minY; j < maxY; j++)
                {
                    var location = new Location(i, j);
                    if (IsInsideTriangle(v1R, v2R, v3R, location))
                    {
                        var newLocation = location + originVector;
                        if (IsValidLocation(newLocation) && newLocation != origin)
                            locations.Add(newLocation);
                    }
                }
            }

            return locations;
        }

        private Vector2 RotateVector(Vector2 vector, GridDirection direction)
        {
            var angle = (float)(Math.PI / 180) * (int)direction;
            var rotationMatrix = Matrix4x4.CreateRotationZ(angle);
            var newVector = Vector2.Transform(vector, rotationMatrix);
            return newVector;
        }

        private static float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        // Is the {point} inside the triangle specified by the vertices {v1}, {v2} and {v3}?
        private static bool IsInsideTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Location point)
        {
            var vs1 = new Vector2(v2.X - v1.X, v2.Y - v1.Y);
            var vs2 = new Vector2(v3.X - v1.X, v3.Y - v1.Y);
            var q = new Vector2(point.X - v1.X, point.Y - v1.Y);

            var s = CrossProduct(q, vs2) / CrossProduct(vs1, vs2);
            var t = CrossProduct(vs1, q) / CrossProduct(vs1, vs2);
            return s >= 0 && t >= 0 && s + t <= 1;
        }
    }
}