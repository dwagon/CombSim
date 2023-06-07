using CombSim;
using CombSim.Characters;
using CombSim.Gear;
using NUnit.Framework;

namespace CombSimTest.Characters
{
    [TestFixture]
    public class RogueTest
    {
        [Test]
        public void TestL1Rogue()
        {
            var r1 = new Rogue("TestRogue", 1, "Test");
            r1.Initialise();
            Assert.AreEqual(11 + 3, r1.ArmourClass()); // Leather & Dex
        }

        [Test]
        public void TestL4Rogue()
        {
            var r4 = new Rogue("TestRogue", 4, "Test");
            r4.Initialise();
            Assert.AreEqual(12 + 4, r4.ArmourClass()); // Leather +1 & Dex
        }

        [Test]
        public void TestMeleeSneakAttack()
        {
            var game = new Game(5, 5);
            var r1 = new Rogue("Test", 1, "Attacker");
            r1.AddEquipment(MeleeWeaponGear.Dagger);
            var dagger = r1.PickActionByName("Dagger");
            var target = new Target();
            game.Add_Creature(r1);
            game.Add_Creature(target);

            game.Move(r1, new Location(3, 3));
            game.Move(target, new Location(3, 4));

            var sa = new Rogue.SneakAttack(new DamageRoll("1d6"));
            Assert.False(sa.HasAdjacentAlly(r1, target));

            // Attack with no advantage using a finesse weapon
            Assert.False(sa.IsSneakAttack((Attack)dagger, r1, target));

            // Attack with advantage using a finesse weapon
            target.AddCondition(ConditionEnum.Restrained);
            Assert.True(sa.IsSneakAttack((Attack)dagger, r1, target));
        }

        [Test]
        public void TestRangedSneakAttack()
        {
            var game = new Game(5, 5);
            var r1 = new Rogue("Test", 1, "Attacker");
            r1.AddEquipment(RangedWeaponGear.Longbow);
            var longbow = r1.PickActionByName("Longbow");
            var target = new Target();
            game.Add_Creature(r1);
            game.Add_Creature(target);

            game.Move(r1, new Location(3, 1));
            game.Move(target, new Location(3, 4));

            var sa = new Rogue.SneakAttack(new DamageRoll("1d6"));

            // Attack with no advantage
            Assert.False(sa.IsSneakAttack((Attack)longbow, r1, target));

            // Attack with advantage using a ranged weapon
            target.AddCondition(ConditionEnum.Restrained);
            Assert.True(sa.IsSneakAttack((Attack)longbow, r1, target));
        }

        private class Target : Creature
        {
            public Target() : base("Target", "Target")
            {
            }
        }
    }
}