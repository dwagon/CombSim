using CombSim;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class AttackTest
    {
        [Test]
        public void TestAttack()
        {
            var game = new Game();
            var ta = new TestAttack();
            var attacker = new TestCritter();
            var victim = new TestCritter();
            game.Add_Creature(attacker);
            game.Add_Creature(victim);
            attacker.AddAction(ta);
            attacker.PickActionByName("TestAttack");
            ta.PerformAction(attacker);
        }

        [Test]
        public void TestStatForAttack()
        {
            var attacker = new TestCritter();
            var attack = new TestAttack();
            Assert.AreEqual(StatEnum.Strength, attack.UseStatForAttack(attacker));
        }

        [Test]
        public void TestFinesseAttack()
        {
            var attacker = new TestCritter();
            var attack = new TestFinesseAttack();
            attacker.Stats[StatEnum.Strength] = new Stat(10);
            attacker.Stats[StatEnum.Dexterity] = new Stat(8);
            Assert.AreEqual(StatEnum.Strength, attack.UseStatForAttack(attacker));
            attacker.Stats[StatEnum.Dexterity] = new Stat(18);
            Assert.AreEqual(StatEnum.Dexterity, attack.UseStatForAttack(attacker));
        }
    }

    public class TestAttack : MeleeAttack
    {
        public TestAttack() : base("TestAttack", new DamageRoll("1d0", DamageTypeEnums.Psychic))
        {
        }
    }

    public class TestFinesseAttack : MeleeAttack
    {
        public TestFinesseAttack() : base("TestAttack", new DamageRoll("1d0", DamageTypeEnums.Psychic))
        {
            Finesse = true;
        }
    }

    public class TestCritter : Creature
    {
        public TestCritter() : base("TestCritter")
        {
        }
    }
}