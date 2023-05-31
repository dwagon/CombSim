﻿using CombSim.Characters;
using CombSim.Monsters;

namespace CombSim
{
    internal class Program
    {
        private Game _game;

        public static void Main(string[] args)
        {
            var pc = new Program();
            for (int i = 0; i < 1; i++)
            {
                pc.SetUp();
                pc.RunGame();
            }
        }

        private void RunGame()
        {
            _game.StartGame();
            _game.RunGame();
            _game.EndGame();
        }

        private void SetUp()
        {
            _game = new Game(80);
            var monsters = "monsters";
            _game.Add_Creature(new Ghoul("Ghoul1", monsters));
            _game.Add_Creature(new Ogre("Ogre1", monsters));
            _game.Add_Creature(new Zombie("Zombie1", monsters));
            _game.Add_Creature(new Skeleton("Skeleton1", monsters));
            _game.Add_Creature(new AirElemental("AirElemental1", monsters));

            var gnolls = "gnolls";
            for (var i = 0; i < 5; i++)
            {
                _game.Add_Creature(new Gnoll($"Gnoll{i}", gnolls));
            }

            var humans = "humans";
            _game.Add_Creature(new Fighter("Fighter", level: 4, humans));
            _game.Add_Creature(new Wizard("Wizard", level: 4, humans));
            _game.Add_Creature(new Warlock("Warlock", level: 3, humans));
            _game.Add_Creature(new Rogue("Rogue", level: 4, humans));
            _game.Add_Creature(new Cleric("Cleric", level: 3, humans));
        }
    }
}