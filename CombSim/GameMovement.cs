// All the parts of the Game class that are about moving around the arena, locations within the arena etc.

using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public partial class Game
    {
        // Return the closest creature to {actor} that is on a different team
        public Creature PickClosestEnemy(Creature actor)
        {
            var enemies = new List<(Creature, float)>();
            foreach (var critter in _combatants)
                if (critter.Team != actor.Team && critter.IsOk())
                    enemies.Add((critter, DistanceTo(actor, critter)));
            if (enemies.Count == 0) return null;
            enemies.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            return enemies.First().Item1;
        }

        // Return the distance between creatures {one} and {two}
        public int DistanceTo(Creature one, Creature two)
        {
            if (!_locations.ContainsKey(one) || !_locations.ContainsKey(two)) return -1;
            return (int)_arena.DistanceBetween(_locations[one], _locations[two]);
        }

        public Creature GetCreatureAtLocation(Location location)
        {
            return (_arena.GetLocation(location) as Creature);
        }

        // Can creatures move into this {location}
        private bool IsWalkable(Location location)
        {
            return _arena.IsClear(location);
        }

        public IEnumerable<Location> GetRouteTowards(Creature mover, Creature target)
        {
            return GetRouteTowards(_locations[mover], _locations[target]);
        }

        public IEnumerable<Location> GetRouteTowards(Location source, Location destination)
        {
            var route = FindPath(source, destination);
            return route;
        }

        public Location GetLocation(Creature creature)
        {
            if (!_locations.ContainsKey(creature)) return null;
            return _locations[creature];
        }

        public List<Location> GetNeighbourLocations(Creature creature)
        {
            return _arena.GetNeighbours(_locations[creature]);
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

        public IEnumerable<Location> GetConeLocations(Creature actor, int coneSize, GridDirection direction)
        {
            return _arena.ConeLocations(actor.GetLocation(), coneSize, direction);
        }

        public Location NextLocationTowards(Creature srcCreature, Creature dstCreature)
        {
            return NextLocationTowards(_locations[srcCreature], _locations[dstCreature]);
        }

        // Move {creature} to new {location}
        public void Move(Creature creature, Location location)
        {
            _arena.Clear(_locations[creature]);
            _locations[creature] = location;
            _arena.Set(location, creature);
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
            foreach (var neighbour in _arena.GetNeighbours(destination))
            {
                var solution = FindRoute(source, neighbour);
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
            fScore[source] = _arena.DistanceBetween(source, destination);
            while (openSet.Any())
            {
                var current = openSet.OrderBy(c => fScore[c]).First();
                if (current == destination) return ReconstructPath(path, current);

                openSet.Remove(current);
                closed.Add(current);
                foreach (var neighbour in _arena.GetNeighbours(current))
                {
                    if (closed.Contains(neighbour) || !IsWalkable(neighbour)) continue;
                    var tentative = gScore[current] + _arena.DistanceBetween(current, neighbour);
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else if (tentative >= (gScore.ContainsKey(neighbour) ? gScore[neighbour] : 999999999)) continue;

                    path[neighbour] = current;
                    gScore[neighbour] = tentative;
                    fScore[neighbour] = gScore[neighbour] + _arena.DistanceBetween(neighbour, destination);
                }
            }

            return null;
        }
    }
}