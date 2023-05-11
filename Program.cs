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
            var k1 = new Kobold("Kobold1");
            _game.Add_Creature(k1);
            var k2 = new Kobold("Kobold2");
            _game.Add_Creature(k2);
            var g1 = new Goblin("Goblin1");
            _game.Add_Creature(g1);
            var g2 = new Goblin("Goblin2");
            _game.Add_Creature(g2);
            var f = new Fighter("Frank");
            _game.Add_Creature(f);
        }
    }
}