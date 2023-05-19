using NUnit.Framework;
using CombSim;

namespace CombSimTest
{
    [TestFixture]
    public class LocationTests
    {
        [Test]
        public void TestLocationCreation()
        {
            var location = new Location(5, 5);
            Assert.AreEqual(location.X, 5);
            Assert.AreEqual(location.Y, 5);
        }

        [Test]
        public void TestEquals()
        {
            var location = new Location(3, 4);
            Assert.AreEqual(location, new Location(3,4));
        }

        [Test]
        public void TestToString()
        {
            var location = new Location(3, 4);
            Assert.AreEqual(location.ToString(), "(3,4)");
        }

        [Test]
        public void TestDistance()
        {
            var locationA = new Location(5, 5);
            var locationB = new Location(5, 7);
            Assert.AreEqual(locationA.DistanceBetween(locationB), 2f);
        }

        [Test]
        public void TestDblEquals()
        {
            var locationA = new Location(5, 5);
            var locationB = new Location(5, 6);
            var locationC = new Location(5, 7);
            Assert.True(locationA == locationB);
            Assert.False(locationA == locationC);
        }
    }
}