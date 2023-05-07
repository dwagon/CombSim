using System;

namespace CombSim
{
    public class Arena
    {
        private readonly int _maxX;
        private readonly int _maxY;
        private readonly object[,] _grid;

        public Arena(int maxX, int maxY)
        {
            _grid = new object[maxX, maxY];
            _maxX = maxX;
            _maxY = maxY;
            for (int x = 0; x < _maxX; x++)
            {
                for (int y = 0; y < _maxY; y++)
                {
                    _grid[x, y] = null;
                }
            }
        }

        // Print the arena out
        public new string ToString()
        {
            string output = "";
            for (int y = 0; y < _maxY; y++)
            {
                for (int x = 0; x < _maxX; x++)
                {
                    if (_grid[x, y] == null)
                    {
                        output += ".";
                    }
                    else
                    {
                        Creature creature = _grid[x, y] as Creature;
                        output += creature?.GetRepr();
                    }
                }
                output+="\n";
            }
            return output;
        }

        // Return an empty spot on the arena
        public bool Pick_Empty_Spot(out int x, out int y)
        {
            x = -1;
            y = -1;
            for (int attempt = 0; attempt < _maxX * _maxY; attempt++)
            {
                Random rnd = new Random();
                x = rnd.Next() % _maxX;
                y = rnd.Next() % _maxY;
                if (_grid[x, y] == null)
                {
                    return true;
                }
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
    }
}