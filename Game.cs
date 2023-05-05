using System.Collections.Generic;

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

        public void RunGame()
        {
            for (int turn = 0; turn < 5; turn++)
            {
                TakeTurn();
            }
        }

        public void TakeTurn()
        {
            foreach (var creature in initiativeOrder)
            {
                creature.TakeTurn();
            }
        }
        
        public void EndGame() {}
        
        public void Add_Creature(Creature creature)
        {
            arena.Pick_Empty_Spot(out int x, out int y);
            arena.Set(x, y, creature);
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
    }
}