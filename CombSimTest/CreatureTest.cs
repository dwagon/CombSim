using CombSim;
using CombSim.Gear;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class CreatureTest
    {
        [Test]
        public void TestBaseArmourClass()
        {
            var creature = new Creature("Test", "TestTeam");
            creature.Stats[StatEnum.Dexterity] = new Stat(11);
            Assert.AreEqual(10, creature.ArmourClass());
        }

        [Test]
        public void TestDexArmourClass()
        {
            var creature = new Creature("Test", "TestTeam");
            creature.Stats[StatEnum.Dexterity] = new Stat(16);
            Assert.AreEqual(10 + 3, creature.ArmourClass());
        }

        [Test]
        public void TestDexAndArmour()
        {
            var creature = new Creature("Test", "TestTeam");
            creature.Stats[StatEnum.Dexterity] = new Stat(16);
            creature.AddEquipment(ArmourGear.Leather);
            Assert.AreEqual(11 + 3, creature.ArmourClass());
        }

        [Test]
        public void TestShieldArmourClass()
        {
            var creature = new Creature("Test", "TestTeam");
            creature.Stats[StatEnum.Dexterity] = new Stat(10);
            creature.AddEquipment(ArmourGear.Shield);
            Assert.AreEqual(10 + 2, creature.ArmourClass());
        }

        [Test]
        public void TestRingArmourClass()
        {
            var creature = new Creature("Test", "TestTeam");
            creature.Stats[StatEnum.Dexterity] = new Stat(18);
            creature.AddEquipment(ArmourGear.Ring);
            Assert.AreEqual(14, creature.ArmourClass());
        }


        [Test]
        public void TestMagicArmourClass()
        {
            var creature = new Creature("Test", "TestTeam");
            creature.Stats[StatEnum.Dexterity] = new Stat(10);
            creature.AddEquipment(ArmourGear.LeatherPlusOne);
            Assert.AreEqual(12, creature.ArmourClass());
        }
    }
}