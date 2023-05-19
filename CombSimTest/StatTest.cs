using CombSim;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class StatTest
    {
        [Test]
        public void BonusTest()
        {
            var stat = new Stat(16);
            Assert.AreEqual(3, stat.Bonus());
        }
    }
}