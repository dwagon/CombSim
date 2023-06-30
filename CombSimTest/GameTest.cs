using CombSim;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class GameTest
    {
        [Test]
        public void TestGetLocation()
        {
            var g = new Game(10, 10);
            var creature = new TestCreature("TestBunny");
            g.Add_Creature(creature);
            var creatureLocation = g.GetLocation(creature);
            Assert.IsNotNull(creatureLocation);
            Assert.AreEqual(creature, g.GetCreatureAtLocation(creatureLocation));
        }

        [Test]
        public void TestMove()
        {
            var g = new Game(10, 10);
            var creature = new TestCreature("TestBunny");
            g.Add_Creature(creature);
            g.Move(creature, new Location(0, 0));
            var creatureLocation = g.GetLocation(creature);
            Assert.AreEqual(creatureLocation, new Location(0, 0));
        }
    }

    public class TestCreature : Creature
    {
        public TestCreature(string name) : base(name, "test")
        {
        }
    }
}