using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace CombSim
{
    public class Arena
    {
        private int _maxX;
        private int _maxY;
        private object[,] _grid;

        public Arena(int max_x, int max_y)
        {
            _grid = new object[max_x, max_y];
            _maxX = max_x;
            _maxY = max_y;
            for (int x = 0; x < _maxX; x++)
            {
                for (int y = 0; y < _maxY; y++)
                {
                    _grid[x, y] = null;
                }
            }
        }

        // Print the arena out
        public void Print()
        {
            Console.WriteLine("");
            for (int y = 0; y < _maxY; y++)
            {
                for (int x = 0; x < _maxX; x++)
                {
                    if (_grid[x, y] == null)
                    {
                        Console.Write(".");
                    }
                    else
                    {
                        Creature creature = _grid[x, y] as Creature;
                        
                        Console.Write(creature.GetRepr());
                    }
                }
                Console.WriteLine("");
            }
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
            int x, y;
            x = location.x;
            y = location.y;
            _grid[x, y] = thing;
        }

        public void Move(Location newplace, Creature creature)
        {
            int x = creature.Location.x;
            int y = creature.Location.y;
            Set(x, y, null);
            Set(newplace, creature);
            creature.SetLocation(newplace);
        }
    }
}