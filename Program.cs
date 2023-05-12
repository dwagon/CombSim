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
            _game = new Game(40);
            for (int i = 0; i < 6; i++)
            {
                var s1 = new Skeleton($"Skeleton{i}");
                _game.Add_Creature(s1);
            }
            var f = new Fighter("Frank");
            _game.Add_Creature(f);
        }
    }
}