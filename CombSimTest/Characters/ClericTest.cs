using CombSim;
using CombSim.Characters;
using NUnit.Framework;

namespace CombSimTest.Characters
{
    [TestFixture]
    public class ClericTest
    {
        [Test]
        public void TestL1Cleric()
        {
            var c = new Cleric("Test", 1, "Test");
            c.Initialise();
            Assert.AreEqual(16, c.ArmourClass());
            Assert.True(c.ToString().Contains("HP: 9/9"));
            Assert.True(c.ToString().Contains("Spells: L1 = 2"));
        }

        [Test]
        public void TestCanCastCantrip()
        {
            var c = new Cleric("Test", 1, "Test");
            c.Initialise();
            Assert.True(c.ToString().Contains("Spells: L1 = 2"));
            var cantrip = new TestCantrip();
            Assert.True(c.CanCastSpell(cantrip));
            c.DoCastSpell(cantrip);
            Assert.True(c.ToString().Contains("Spells: L1 = 2"));
        }

        [Test]
        public void TestCastingL1Spell()
        {
            var c = new Cleric("Test", 1, "Test");
            c.Initialise();
            Assert.True(c.ToString().Contains("Spells: L1 = 2"));
            var l1spell = new TestL1Spell();
            Assert.True(c.CanCastSpell(l1spell));
            c.DoCastSpell(l1spell);
            Assert.True(c.ToString().Contains("Spells: L1 = 1"));
            c.DoCastSpell(l1spell);
            Assert.True(c.ToString().Contains("Spells: L1 = 0"));
            Assert.False(c.CanCastSpell(l1spell));
        }

        private class TestCantrip : Spell
        {
            public TestCantrip() : base("Test Cantrip", 0, ActionCategory.Action)
            {
            }
        }

        private class TestL1Spell : Spell
        {
            public TestL1Spell() : base("Test L1", 1, ActionCategory.Action)
            {
            }
        }
    }
}