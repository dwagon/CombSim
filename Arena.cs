using System;
using System.Collections.Generic;

namespace CombSim
{
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

        public void Set(Location location, Creature thing)
        {
            var x = location.x;
            var y = location.y;
            _grid[x, y] = thing;
        }

        public void Clear(Location location)
        {
            Set(location, null);
        }

        public bool IsClear(Location location)
        {
            return _grid[location.x, location.y] == null;
        }

        public List<Location> GetNeighbours(Location location)
        {
            var neighbours = new List<Location>();
            for (var x = Math.Max(0, location.x - 1); x <= Math.Min(_maxX - 1, location.x + 1); x++)
            for (var y = Math.Max(0, location.y - 1); y <= Math.Min(_maxY - 1, location.y + 1); y++)
            {
                if (x == location.x && y == location.y) continue;
                neighbours.Add(new Location(x, y));
            }

            return neighbours;
        }
    }
}