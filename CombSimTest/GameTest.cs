using System;
using System.Collections.Generic;
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

        [Test]
        public void TestConeLocations()
        {
            var g = new Game(10, 10);
            var creature = new TestCreature("TestBunny");
            g.Add_Creature(creature);
            g.Move(creature, new Location(5, 5));
            var locations = g.GetConeLocations(creature, 3, GridDirection.E);
            Console.WriteLine(String.Join(", ", locations));
            /*    x
             * oxxx
             *   xx
             */
            Assert.AreEqual(locations, new HashSet<Location>()
            {
                new Location(6, 5), new Location(7, 5), new Location(7, 6),
                new Location(8, 4), new Location(8, 5), new Location(8, 6)
            });
        }
    }

    public class TestCreature : Creature
    {
        public TestCreature(string name) : base(name, "test")
        {
        }
    }
}