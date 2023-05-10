using System;
using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public class Game
    {
        private readonly Arena _arena;
        private readonly List<Creature> _combatants;
        private readonly Dictionary<Creature, Location> _locations;
        private List<Creature> _initiativeOrder;

        public Game(int maxX = 20, int maxY = 10)
        {
            _arena = new Arena(maxX, maxY);
            _combatants = new List<Creature>();
            _locations = new Dictionary<Creature, Location>();
        }

        public void StartGame()
        {
            _initiativeOrder = GetInitiativeOrder();
            foreach (var creature in _combatants) creature.Initialise();
        }

        // Return the next location closer to the {destination}
        private Location NextLocationTowards(Location source, Location destination)
        {
            var route = FindPath(source, destination);
            return route?.First();
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
            Console.WriteLine(_arena);
            while (!IsEndOfGame()) TakeTurn();
        }

        private bool IsEndOfGame()
        {
            var numSides = 0;

            var sides = new Dictionary<string, int>();
            foreach (var combatant in _combatants)
            {
                if (!sides.ContainsKey(combatant.Team)) sides[combatant.Team] = 0;
                if (combatant.IsAlive()) sides[combatant.Team]++;
            }

            foreach (var side in sides.Keys)
                if (sides[side] > 0)
                    numSides++;
            return numSides <= 1;
        }

        private void TakeTurn()
        {
            Console.WriteLine("\n#####################################################");
            foreach (var creature in _initiativeOrder) creature.TakeTurn();
            Console.WriteLine(_arena.ToString());
            foreach (var creature in _combatants) Console.WriteLine(creature.ToString());
        }

        public void EndGame()
        {
        }

        public void Add_Creature(Creature creature)
        {
            _arena.Pick_Empty_Spot(out var x, out var y);
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

            tmp.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            foreach (var critter in tmp) order.Add(critter.Item1);
            return order;
        }

        // Return the closest creature to {actor} that is on a different team
        public Creature PickClosestEnemy(Creature actor)
        {
            var enemies = new List<(Creature, float)>();
            foreach (var critter in _combatants)
                if (critter.Team != actor.Team && critter.IsAlive())
                    enemies.Add((critter, DistanceTo(actor, critter)));
            enemies.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            return enemies.First().Item1;
        }

        // Return the distance between creatures {one} and {two}
        public int DistanceTo(Creature one, Creature two)
        {
            return (int)_locations[one].DistanceBetween(_locations[two]);
        }

        // Move {creature} to new {location}
        public void Move(Creature creature, Location location)
        {
            _arena.Clear(_locations[creature]);
            _locations[creature] = location;
            _arena.Set(location, creature);
        }

        // Can creatures move into this {location}
        private bool IsWalkable(Location location)
        {
            return _arena.IsClear(location);
        }

        private IEnumerable<Location> ReconstructPath(Dictionary<Location, Location> path, Location current)
        {
            var totalPath = new List<Location> { current };

            while (path.ContainsKey(current))
            {
                current = path[current];
                totalPath.Add(current);
            }

            totalPath.Reverse();
            totalPath.RemoveAt(0);

            return totalPath;
        }

        // Return route from {source} to a neighbour of {destination}
        // {destination} is never walkable because the enemy is there.
        private IEnumerable<Location> FindPath(Location source, Location destination)
        {
            var bestLength = 999999;
            Location bestNeighbour = null;
            IEnumerable<Location> solution = null;
            foreach (var neighbour in _arena.GetNeighbours(destination))
            {
                solution = FindRoute(source, neighbour);
                if (solution?.Count() < bestLength)
                {
                    bestNeighbour = neighbour;
                    bestLength = solution.Count();
                }
            }

            return FindRoute(source, bestNeighbour);
        }

        // Return route from {source} to {destination}
        private IEnumerable<Location> FindRoute(Location source, Location destination)
        {
            var closed = new List<Location>();
            var openSet = new List<Location> { source };
            var path = new Dictionary<Location, Location>();
            var gScore = new Dictionary<Location, float>();
            gScore[source] = 0;

            var fScore = new Dictionary<Location, float>();
            fScore[source] = source.DistanceBetween(destination);
            while (openSet.Any())
            {
                var current = openSet.OrderBy(c => fScore[c]).First();
                if (current == destination) return ReconstructPath(path, current);

                openSet.Remove(current);
                closed.Add(current);
                foreach (var neighbour in _arena.GetNeighbours(current))
                {
                    if (closed.Contains(neighbour) || !IsWalkable(neighbour)) continue;
                    var tentative = gScore[current] + current.DistanceBetween(neighbour);
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else if (tentative >= (gScore.ContainsKey(neighbour) ? gScore[neighbour] : 999999999)) continue;

                    path[neighbour] = current;
                    gScore[neighbour] = tentative;
                    fScore[neighbour] = gScore[neighbour] + neighbour.DistanceBetween(destination);
                }
            }

            return null;
        }
    }
}