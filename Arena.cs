using System;
using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public class Arena
    {
        private int _max_x;
        private int _max_y;
        private object[,] _grid;
        private List<Creature> _combatants;

        public Arena(int max_x, int max_y)
        {
            _grid = new object[max_x, max_y];
            _combatants = new List<Creature>();
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

        public void Add_Creature(Creature creature)
        {
            Pick_Empty_Spot(out int x, out int y);
            _grid[x, y] = creature;
            _combatants.Add(creature);
        }

        // Return an empty spot on the arena
        private bool Pick_Empty_Spot(out int x, out int y)
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
        
        // Calculate and return initiative order
        public List<Creature> GetInitiativeOrder()
        {
            var tmp = new List<(Creature, int)>();
            var order = new List<Creature>();

            foreach (var creature in _combatants)
            {
                (Creature, int) init = (creature, creature.RollInitiative());
                tmp.Add(init);
            }

            tmp.Sort((x,y) => x.Item2.CompareTo(y.Item2));
            foreach (var critter in tmp)
            {
                order.Add(critter.Item1);
            }
            return order;
        }
    }
}