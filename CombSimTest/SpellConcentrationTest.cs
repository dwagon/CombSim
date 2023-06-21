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
            Assert.AreEqual(null, caster.ConcentratingOnSpell);
            action.PerformAction(caster);
            Assert.AreEqual("Test Spell", caster.ConcentratingOn().Name());
        }

        [Test]
        public void IsConcentrationTest()
        {
            var spell = new TestSpell();
            Assert.True(spell.IsConcentration());
        }

        [Test]
        public void ChangeConcentration()
        {
            var caster = new TestCaster();
            var spell = caster.PickActionByName("Test Spell");
            spell.PerformAction(caster);
            Assert.AreEqual("Test Spell", caster.ConcentratingOn().Name());
            var spell2 = caster.PickActionByName("Test Spell2");
            spell2.PerformAction(caster);
            Assert.AreEqual("Test Spell2", caster.ConcentratingOn().Name());
        }
    }

    public class TestCaster : Character
    {
        public TestCaster() : base("Test Caster", 1)
        {
            AddSpell(new TestSpell());
            AddSpell(new TestSpell2());
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

    public class TestSpell2 : Spell
    {
        public TestSpell2() : base("Test Spell2", 0, ActionCategory.Bonus)
        {
            Concentration = true;
        }

        protected override void DoAction(Creature actor)
        {
        }
    }
}