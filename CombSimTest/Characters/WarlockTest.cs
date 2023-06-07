using CombSim.Characters;
using NUnit.Framework;

namespace CombSimTest.Characters
{
    [TestFixture]
    public class WarlockTest
    {
        [Test]
        public void TestL1Warlock()
        {
            var w = new Warlock("Test", 1, "Test");
            w.Initialise();
            Assert.AreEqual(14, w.ArmourClass());
            Assert.True(w.ToString().Contains("HP: 10/10"));
            Assert.True(w.ToString().Contains("Spells: 1"));

            Assert.IsNotNull(w.GetSpell("Thunderclap"));

            var eb = w.GetSpell("Eldritch Blast");
            Assert.AreEqual(0, eb.Level);
            Assert.AreEqual(24, eb.Reach);
        }

        [Test]
        public void TestL2Warlock()
        {
            var w = new Warlock("Test", 2, "Test");
            w.Initialise();
            Assert.AreEqual(14, w.ArmourClass());
            Assert.True(w.ToString().Contains("HP: 18/18"));
            Assert.True(w.ToString().Contains("Spells: 2"));

            var eb = w.GetSpell("Eldritch Blast");
            Assert.AreEqual(0, eb.Level);
            Assert.AreEqual(60, eb.Reach);
        }
    }
}