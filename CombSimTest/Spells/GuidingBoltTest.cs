using System;
using CombSim;
using CombSim.Spells;
using NUnit.Framework;

namespace CombSimTest.Spells
{
    [TestFixture]
    public class GuidingBoltTest
    {
        private readonly Caster _caster = new Caster();
        private readonly Target _target = new Target();
        private Game _game;

        [Test]
        public void TestCast()
        {
            SetUpTest();
            var gb = _caster.PickActionByName("Guiding Bolt");
            gb.PerformAction(_caster);
            Assert.True(_target.HasEffect("Guiding Bolt Effect"));
            Assert.True(_target.HasAdvantageAgainstMe(_caster));
        }

        [Test]
        public void TestEffectEnding()
        {
            SetUpTest();
            var gb = _caster.PickActionByName("Guiding Bolt");
            gb.PerformAction(_caster);
            Assert.True(_target.HasEffect("Guiding Bolt Effect"));
            _caster.TestReset();
            var tb = _caster.PickActionByName("TestBow");
            Console.WriteLine($"// Test Bow = {tb.Name()}");
            tb.PerformAction(_caster);
            Assert.False(_target.HasEffect("Guiding Bolt Effect"));
        }

        private void SetUpTest()
        {
            _game = new Game(10, 10);
            _game.Add_Creature(_caster);
            _caster.Initialise();
            _game.Add_Creature(_target);
            _target.Initialise();
        }

        private class Caster : Creature
        {
            public Caster() : base("TestCaster", "A")
            {
                SpellCastingAbility = StatEnum.Wisdom;
                Stats.Add(StatEnum.Wisdom, new Stat(99)); // Ensure hit
                Stats.Add(StatEnum.Dexterity, new Stat(99));
                AddSpell(new GuidingBolt(2));
                AddEquipment(TestBow);
            }

            public override bool CanCastSpell(Spell spell)
            {
                return true;
            }

            public override void DoCastSpell(Spell spell)
            {
            }
        }

        private class Target : Creature
        {
            public Target() : base("TestTarget", "B")
            {
                Stats.Add(StatEnum.Dexterity, new Stat(1));
                MaxHitPoints = 9999; // Don't die - causes tests to be weird
            }
        }

        private static readonly RangedWeapon TestBow = new RangedWeapon("TestBow",
            new DamageRoll("1d8", DamageTypeEnums.Piercing), 80 / 5, 320 / 5);
    }
}