using System;
using System.Collections.Generic;
using System.Data;

namespace CombSim
{
    internal class Program
    {
        private Game _game;
        
        public static void Main(string[] args)
        {
            Program pc = new Program();
            pc.SetUp();
            pc.RunGame();
        }

        private void RunGame()
        {
            _game.StartGame();
            _game.RunGame();
            _game.EndGame();
        }

        private void SetUp()
        {
            _game = new Game(20, 10);
            Kobold k1 = new Kobold("Kobold1");
            _game.Add_Creature(k1);
            Kobold k2 = new Kobold("Kobold2");
            _game.Add_Creature(k2);
            Kobold k3 = new Kobold("Kobold3");
            _game.Add_Creature(k3);
            Fighter f = new Fighter("Frank");
            _game.Add_Creature(f);
        }
    }
}