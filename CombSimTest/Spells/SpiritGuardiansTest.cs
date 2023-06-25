using CombSim;
using CombSim.Spells;
using NUnit.Framework;

namespace CombSimTest.Spells
{
    [TestFixture]
    public class SpiritGuardiansTest
    {
        private Game _game;
        private readonly Creature _caster = new Caster();
        private readonly Creature _target = new Target();

        [Test]
        public void TestCast()
        {
            SetupTest();
            var sg = _caster.PickActionByName("Spirit Guardians");
            sg.PerformAction(_caster);
            Assert.True(_caster.HasEffect("Spirit Guardians"));
            Assert.AreEqual(sg, _caster.ConcentratingOn());
        }

        [Test]
        public void TestDamage()
        {
            SetupTest();
            var sg = _caster.PickActionByName("Spirit Guardians");
            sg.PerformAction(_caster);
            var hpd = _target.HitPointsDown();
            Assert.Greater(hpd, 0);
            _target.MoveWithinReachOfCreature(1, _caster);
            Assert.AreEqual(hpd, _target.HitPointsDown()); // Take no more damage
        }

        private void SetupTest()
        {
            {
                _game = new Game(10, 10);
                _game.Add_Creature(_caster);
                _caster.Initialise();
                _game.Add_Creature(_target);
                _target.Initialise();
                _game.Move(_caster, new Location(3, 3));
                _game.Move(_target, new Location(6, 3));
            }
        }

        private class Caster : Creature
        {
            public Caster() : base("TestCaster", "A")
            {
                SpellCastingAbility = StatEnum.Wisdom;
                Stats.Add(StatEnum.Strength, new Stat(10));
                Stats.Add(StatEnum.Wisdom, new Stat(99)); // Ensure hit
                AddSpell(new SpiritGuardians());
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
                Stats.Add(StatEnum.Strength, new Stat(1));
                Stats.Add(StatEnum.Wisdom, new Stat(1));
                Stats.Add(StatEnum.Dexterity, new Stat(1));
                MaxHitPoints = 9999; // Don't die - causes tests to be weird
            }
        }
    }
}