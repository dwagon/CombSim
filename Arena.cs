using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace CombSim
{
    public class Arena
    {
        private int _max_x;
        private int _max_y;
        private object[,] _grid;

        public Arena(int max_x, int max_y)
        {
            _grid = new object[max_x, max_y];
            _max_x = max_x;
            _max_y = max_y;
            for (int x = 0; x < _max_x; x++)
            {
                for (int y = 0; y < _max_y; y++)
                {
                    _grid[x, y] = null;
                }
            }
        }

        // Print the arena out
        public void Print()
        {
            Console.WriteLine("");
            for (int y = 0; y < _max_y; y++)
            {
                for (int x = 0; x < _max_x; x++)
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

        // Return the closest creature to {actor} that is on a different team
        public Creature PickClosestEnemy(Creature actor)
        {
            return null;
        }

        // Return an empty spot on the arena
        public bool Pick_Empty_Spot(out int x, out int y)
        {
            x = -1;
            y = -1;
            for (int attempt = 0; attempt < _max_x * _max_y; attempt++)
            {
                Random rnd = new Random();
                x = rnd.Next() % _max_x;
                y = rnd.Next() % _max_y;
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
    }
}