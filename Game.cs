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

        public Game(int max_x = 20, int max_y = 10)
        {
            arena = new Arena(max_x, max_y);
            _combatants = new List<Creature>();
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
                    return new Location(source.x - 1, source.x - 1);
                return new Location(source.x - 1, source.x + 1);
            }

            if (source.y < destination.y)
                return new Location(source.x + 1, source.x - 1);
            return new Location(source.x +1, source.x + 1);
        }

        public void RunGame()
        {
            for (int turn = 0; turn < 5; turn++)
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
            return one.Location.DistanceBetween(two.Location);
        }
    }
}