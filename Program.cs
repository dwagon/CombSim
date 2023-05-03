using System;
using System.Collections.Generic;

namespace CombSim
{
    internal class Program
    {
        private Arena arena = new Arena(20, 10);
        private List<Creature> initiativeOrder;
        
        public static void Main(string[] args)
        {
            Program pc = new Program();
            pc.SetUp();
            for (int turn = 0; turn < 5; turn++)
            {
                pc.TakeTurn();
            }
        }

        private void SetUp()
        {
            Kobold k = new Kobold("Kobold");
            arena.Add_Creature(k);
            Fighter f = new Fighter("Frank");
            arena.Add_Creature(f);
            initiativeOrder = arena.GetInitiativeOrder();
        }

        private void TakeTurn()
        {  
            arena.Print();
            foreach (var creature in initiativeOrder)
            {
                creature.TakeTurn();
            }
        }
    }
}