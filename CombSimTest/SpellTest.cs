using CombSim;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class SpellTest
    {
        [Test]
        public void TestSpell()
        {
            var spell = new TestToHitSpell();
            Assert.AreEqual(0, spell.Level);
            Assert.AreEqual(10, spell.Reach);
        }
    }

    public class TestToHitSpell : Spell
    {
        public TestToHitSpell() : base("Test", 0, ActionCategory.Action)
        {
            Reach = 50 / 5;
            DmgRoll = new DamageRoll("1d8", DamageTypeEnums.Acid);
        }
    }
}