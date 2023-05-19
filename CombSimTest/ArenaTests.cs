using System.Collections.Generic;
using CombSim;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class ArenaTests
    {
        [Test]
        public void TestArenaCreation()
        {
            Arena a = new Arena(10, 10);
            Assert.True(a.IsClear(new Location(5, 5)));
        }

        [Test]
        public void TestNeighbours()
        {
            Arena a = new Arena(10, 10);
            var TLneighbours = a.GetNeighbours(new Location(0, 0));
            Assert.AreEqual(TLneighbours.Count, 3);
            Assert.AreEqual(TLneighbours,
                new List<Location> { new Location(0, 1), new Location(1, 0), new Location(1, 1) });

            var BRneighbours = a.GetNeighbours(new Location(9, 9));
            Assert.AreEqual(BRneighbours.Count, 3);
            Assert.AreEqual(BRneighbours,
                new List<Location> { new Location(8, 8), new Location(8, 9), new Location(9, 8) });

            var Cneighbours = a.GetNeighbours(new Location(5, 5));
            Assert.AreEqual(Cneighbours.Count, 8);
            Assert.AreEqual(Cneighbours, new List<Location>
            {
                new Location(4, 4), new Location(4, 5), new Location(4, 6),
                new Location(5, 4), new Location(5, 6),
                new Location(6, 4), new Location(6, 5), new Location(6, 6)
            });
        }

        [Test]
        public void TestDistance()
        {
            Arena a = new Arena(10, 10);

            var locationA = new Location(5, 5);
            var locationB = new Location(5, 7);
            Assert.AreEqual(a.DistanceBetween(locationA, locationB), 2f);
        }
    }
}