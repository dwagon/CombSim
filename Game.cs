using System;
using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public class Game
    {
        private Arena arena;
        private List<Creature> initiativeOrder;
        private List<Creature> _combatants;
        private Dictionary<Creature, Location> _locations;

        public Game(int max_x = 20, int max_y = 10)
        {
            arena = new Arena(max_x, max_y);
            _combatants = new List<Creature>();
            _locations = new Dictionary<Creature, Location>();
        }

        public void StartGame()
        {
            initiativeOrder = GetInitiativeOrder();
        }

        // Return the next location closer to the {destination}
        public Location NextLocationTowards(Location source, Location destination)
        {
            if (source.x < destination.x)
            {
                if (source.y < destination.y)
                    return new Location(source.x + 1, source.y - 1);
                return new Location(source.x + 1, source.y);
            }

            if (source.y < destination.y)
                return new Location(source.x - 1, source.y - 1);
            return new Location(source.x - 1, source.y);
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
            for (int turn = 0; turn < 1; turn++)
            {
                TakeTurn();
            }
        }

        private void TakeTurn()
        {
            foreach (var creature in initiativeOrder)
            {
                creature.TakeTurn();
            }
            arena.Print();
        }
        
        public void EndGame() {}
        
        public void Add_Creature(Creature creature)
        {
            arena.Pick_Empty_Spot(out int x, out int y);
            arena.Set(x, y, creature);
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
                if (critter.Team != actor.Team)
                {
                    enemies.Add((critter, DistanceTo(actor, critter)));
                }
            }
            enemies.Sort((a,b) =>a.Item2.CompareTo(b.Item2));

            return enemies.Last().Item1;
        }

        // Return the distance between creatures {one} and {two}
        public float DistanceTo(Creature one, Creature two)
        {
            return _locations[one].DistanceBetween(_locations[two]);
        }
        
        // Move {creature} to new {location}
        public void Move(Creature creature, Location location)
        {
            arena.Clear(_locations[creature]);
            _locations[creature] = location;
            arena.Set(location, creature);
        }
    }
}