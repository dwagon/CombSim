using System;
using System.Collections.Generic;

namespace CombSim
{
    public partial class Game
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

        public void RunGame()
        {
            var turn = 0;
            Console.WriteLine(_arena.ToString());
            foreach (var creature in _combatants) Console.WriteLine(creature.ToString());

            while (!IsEndOfGame() && turn < 30) TakeTurn(turn++);
            EndGame();
        }

        private void EndOfGameReport()
        {
            var damageInflicted = new List<(int, string)>();
            Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            // Sort by who has done the most damage
            foreach (var creature in _combatants)
            {
                var report = creature.ToString();
                report += "\tInflicted: ";
                report += creature.DamageReport(out var total);
                damageInflicted.Add((total, report));
            }

            damageInflicted.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            foreach (var report in damageInflicted)
            {
                Console.WriteLine(report.Item2);
            }
        }

        private bool IsEndOfGame()
        {
            var numSides = 0;

            var sides = new Dictionary<string, int>();
            foreach (var combatant in _combatants)
            {
                if (!sides.ContainsKey(combatant.Team)) sides[combatant.Team] = 0;
                if (combatant.IsOk()) sides[combatant.Team]++;
            }

            foreach (var side in sides.Keys)
                if (sides[side] > 0)
                    numSides++;
            return numSides <= 1;
        }

        public IEnumerable<Creature> GetCreaturesInCircle(Location origin, int radius)
        {
            var creatures = new List<Creature>();
            foreach (var location in _arena.CircleLocations(origin, radius))
            {
                if (_arena.GetLocation(location) != null)
                {
                    creatures.Add((Creature)_arena.GetLocation(location));
                }
            }

            return creatures;
        }

        private void TakeTurn(int turn)
        {
            Console.WriteLine($"\n# {turn} ##########################################################################");
            foreach (var creature in _initiativeOrder)
            {
                creature.TakeTurn();
                if (creature.IsOk()) GameReport();
            }

            GameReport();
        }

        private void GameReport()
        {
            Console.WriteLine(_arena.ToString());
            foreach (var creature in _combatants) Console.WriteLine(creature.ToString());
        }

        public void EndGame()
        {
            EndOfGameReport();
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

        // Return all allies - even unconscious ones
        public List<Creature> GetAllAllies(Creature actor)
        {
            var friends = new List<Creature>();
            foreach (var critter in _combatants)
            {
                if (critter.Team == actor.Team && critter.IsAlive())
                    friends.Add(critter);
            }

            return friends;
        }

        // Return all enemies - even unconscious ones
        public List<Creature> GetAllEnemies(Creature actor)
        {
            var enemies = new List<Creature>();
            foreach (var critter in _combatants)
            {
                if (critter.Team != actor.Team && critter.IsAlive())
                    enemies.Add(critter);
            }

            return enemies;
        }

        public void CreatureFallenUnconscious(Creature cause, Creature victim, Action action)
        {
            Creature.OnAnyBeingKilled?.Invoke(this, new Creature.OnAnyBeingKilledEventArgs
            {
                Source = cause,
                Victim = victim,
                Action = action
            });
        }

        // {creature} no longer exists (i.e. died), remove them from the game
        public void Remove(Creature creature)
        {
            try
            {
                _arena.Clear(_locations[creature]);
                _locations.Remove(creature);
            }
            catch (KeyNotFoundException)
            {
                // Ignore as the same creature can "die" multiple times - e.g Scorching Ray where it is near death
            }
        }
    }
}