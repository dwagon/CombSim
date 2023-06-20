using CombSim;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class SpellConcentrationTest
    {
        [Test]
        public void TestCasting()
        {
            var caster = new TestCaster();
            var action = caster.PickActionByName("Test Spell");
            Assert.AreEqual(null, caster._concentration);
            action.PerformAction(caster);
            Assert.AreEqual("Test Spell", caster.ConcentratingOn().Name());
        }

        [Test]
        public void IsConcentrationTest()
        {
            var spell = new TestSpell();
            Assert.True(spell.IsConcentration());
        }
    }

    public class TestCaster : Character
    {
        public TestCaster() : base("Test Caster", 1)
        {
            AddSpell(new TestSpell());
        }

        public override bool CanCastSpell(Spell spell)
        {
            return true;
        }

        public override void DoCastSpell(Spell spell)
        {
        }
    }

    public class TestSpell : Spell
    {
        public TestSpell() : base("Test Spell", 0, ActionCategory.Bonus)
        {
            Concentration = true;
        }

        protected override void DoAction(Creature actor)
        {
        }
    }
}