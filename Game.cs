using System;
using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public class Game
    {
        private readonly Arena _arena;
        private List<Creature> _initiativeOrder;
        private readonly List<Creature> _combatants;
        private readonly Dictionary<Creature, Location> _locations;

        public Game(int maxX = 20, int maxY = 10)
        {
            _arena = new Arena(maxX, maxY);
            _combatants = new List<Creature>();
            _locations = new Dictionary<Creature, Location>();
        }

        public void StartGame()
        {
            _initiativeOrder = GetInitiativeOrder();
            foreach (var creature in _combatants)
            {
                creature.Initialise();
            }
        }

        // Return the next location closer to the {destination}
        private Location NextLocationTowards(Location source, Location destination)
        {
            if (source.x < destination.x)
            {
                if (source.y < destination.y) return new Location(source.x + 1, source.y + 1);
                if (source.y > destination.y) return new Location(source.x + 1, source.y - 1);
                return new Location(source.x + 1, source.y);
            }

            if (source.x > destination.x)
            {
                if (source.y < destination.y) return new Location(source.x - 1, source.y + 1);
                if (source.y > destination.y) return new Location(source.x - 1, source.y - 1);
                return new Location(source.x - 1, source.y);
            }

            return new Location(source.x, source.y);
        }
        
        public Location NextLocationTowards(Creature srcCreature, Location destination)
        {
            return NextLocationTowards(_locations[srcCreature], destination);
        }
        
        public Location NextLocationTowards(Creature srcCreature, Creature dstCreature)
        {
            return NextLocationTowards(_locations[srcCreature], _locations[dstCreature]);
        }

        public Location GetLocation(Creature creature)
        {
            return _locations[creature];
        }

        public void RunGame()
        {
            Console.WriteLine(_arena.ToString());
            while(!IsEndOfGame())
            {
                TakeTurn();
            }
        }

        private bool IsEndOfGame()
        {
            int numSides = 0;

            Dictionary<string, int> sides = new Dictionary<string, int>();
            foreach (var combatant in _combatants)
            {
                if (!sides.ContainsKey(combatant.Team)) sides[combatant.Team] = 0;
                if (combatant.IsAlive()) sides[combatant.Team]++;
            }
            foreach (var side in sides.Keys)
            {
                if (sides[side] > 0) numSides++;
            }
            return (numSides <= 1);
        }

        private void TakeTurn()
        {
            foreach (var creature in _initiativeOrder)
            {
                creature.TakeTurn();
            }
            Console.WriteLine(_arena.ToString());
            foreach (var creature in _combatants)
            {
                Console.WriteLine(creature.ToString());
            }
        }
        
        public void EndGame() {}
        
        public void Add_Creature(Creature creature)
        {
            _arena.Pick_Empty_Spot(out int x, out int y);
            _arena.Set(x, y, creature);
            creature.SetGame(this);
            _combatants.Add(creature);
            _locations.Add(creature, new Location(x, y));
        }

        // Calculate and return initiative order
        private List<Creature> GetInitiativeOrder()
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
        
        // Return the closest creature to {actor} that is on a different team
        public Creature PickClosestEnemy(Creature actor)
        {
            List<(Creature, float)> enemies = new List<(Creature, float)>();
            foreach (var critter in _combatants)
            {
                if (critter.Team != actor.Team && critter.IsAlive())
                {
                    enemies.Add((critter, DistanceTo(actor, critter)));
                }
            }
            enemies.Sort((a,b) =>a.Item2.CompareTo(b.Item2));
            Console.WriteLine($"enemies={String.Join(", ", enemies)} = {enemies.First().Item1}");
            return enemies.First().Item1;
        }

        // Return the distance between creatures {one} and {two}
        public float DistanceTo(Creature one, Creature two)
        {
            return _locations[one].DistanceBetween(_locations[two]);
        }
        
        // Move {creature} to new {location}
        public void Move(Creature creature, Location location)
        {
            _arena.Clear(_locations[creature]);
            _locations[creature] = location;
            _arena.Set(location, creature);
        }
    }
}