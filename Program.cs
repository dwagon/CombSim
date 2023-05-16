using CombSim.Characters;
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
            //_game.Add_Creature(new Ghoul("Ghoul1", "monsters"));
            // _game.Add_Creature(new Ghoul("Ghoul2", "monsters"));
            _game.Add_Creature(new Ogre("Ogre1", "monsters"));
            _game.Add_Creature(new Zombie("Zombie1", "monsters"));
            _game.Add_Creature(new Zombie("Zombie2", "monsters"));
            _game.Add_Creature(new Zombie("Zombie3", "monsters"));

            _game.Add_Creature( new Fighter("Frank", level: 2, "humans"));
            _game.Add_Creature( new Wizard("Wizard", level: 3, "humans"));
        }
    }
}