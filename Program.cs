using System;
using System.Collections.Generic;
using System.Data;

namespace CombSim
{
    internal class Program
    {
        private Game game;
        
        public static void Main(string[] args)
        {
            Program pc = new Program();
            pc.SetUp();
            pc.RunGame();
        }

        private void RunGame()
        {
            game.StartGame();
            game.RunGame();
            game.EndGame();
        }

        private void SetUp()
        {
            game = new Game(20, 10);
            Kobold k = new Kobold("Kobold");
            game.Add_Creature(k);
            Fighter f = new Fighter("Frank");
            game.Add_Creature(f);
        }
    }
}