using System;
using CombSim;
using CombSim.Gear;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class HeuristicTest
    {
        Game _game;
        Creature _attacker;
        Creature _target;

        [Test]
        public void MeleeAttack()
        {
            SetUp();
            var attack = new TestMeleeAttack();
            var result = attack.GetHeuristic(_attacker, out var reason);
            Console.WriteLine($"reason = {reason}");
            Assert.AreEqual(6, result); // Mace Damage is d6
            Assert.True(reason.Contains("Max Damage 6"));
        }

        [Test]
        public void RangedAttack()
        {
            SetUp(false);
            var attack = new TestRangedAttack();
            var result = attack.GetHeuristic(_attacker, out var reason);
            Console.WriteLine($"reason = {reason}");
            Assert.AreEqual(8, result); // Light Crossbow is 1d8
        }

        [Test]
        public void RangedAttackDisadvantage()
        {
            SetUp();
            var attack = new TestRangedAttack();
            var result = attack.GetHeuristic(_attacker, out var reason);
            Console.WriteLine($"reason = {reason}");
            Assert.AreEqual(4, result); // Disadvantage is 1/2 of LightCrossbow d8
            Assert.True(reason.Contains("Disadvantage"));
        }

        [Test]
        public void MagicMeleeAttack()
        {
            SetUp();
            var attack = new TestMagicMeleeAttack();
            var result = attack.GetHeuristic(_attacker, out var reason);
            Console.WriteLine($"reason = {reason}");
            Assert.AreEqual(7, result); // Max mace damage of 6 +1 for magic
            Assert.True(reason.Contains("6+1"));
        }

        private void SetUp(bool adjacent = true)
        {
            _game = new Game(10, 10);
            _attacker = new Attacker();
            _target = new Target();
            _game.Add_Creature(_attacker);
            _game.Add_Creature(_target);
            _game.Move(_attacker, new Location(5, 5));
            if (adjacent)
            {
                _game.Move(_target, new Location(6, 5));
            }
            else
            {
                _game.Move(_target, new Location(8, 5));
            }
        }

        private class TestRangedAttack : RangedAttack
        {
            public TestRangedAttack() : base(RangedWeaponGear.LightCrossbow)
            {
            }
        }

        private class TestMeleeAttack : MeleeAttack
        {
            public TestMeleeAttack() : base(MeleeWeaponGear.Mace)
            {
            }
        }

        private class TestMagicMeleeAttack : MeleeAttack
        {
            public TestMagicMeleeAttack() : base(MeleeWeaponGear.MacePlusOne)
            {
            }
        }

        private class Attacker : Creature
        {
            public Attacker() : base("TestAttacker", "A")
            {
                Stats.Add(StatEnum.Strength, new Stat(10));
                Stats.Add(StatEnum.Dexterity, new Stat(10));
            }
        }

        private class Target : Creature
        {
            public Target() : base("TestTarget", "B")
            {
                Stats.Add(StatEnum.Dexterity, new Stat(10));
            }
        }
    }
}