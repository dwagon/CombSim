using System;
using CombSim;
using CombSim.Characters;
using NUnit.Framework;

namespace CombSimTest.Characters
{
    [TestFixture]
    public class FighterTest
    {
        private string GearList(Character character)
        {
            var gearList = character.GetGearList();
            return String.Join(", ", gearList);
        }

        [Test]
        public void TestL1Fighter()
        {
            var f = new Fighter("Test", 1, "Test");
            f.Initialise();
            Assert.AreEqual(21, f.ArmourClass); // Plate + Shield + Defense Fighting Style
            Assert.True(f.ToString().Contains("HP: 12/12"));
        }

        [Test]
        public void TestL2Fighter()
        {
            var f = new Fighter("Test", 2, "Test");
            f.Initialise();
            Console.WriteLine($"F2={GearList(f)}");
            Assert.AreEqual(22, f.ArmourClass); // Plate + Shield+1 + Defense Fighting Style
            Assert.True(f.ToString().Contains("HP: 20/20"));
        }

        [Test]
        public void TestL3Fighter()
        {
            var f = new Fighter("Test", 3, "Test");
            f.Initialise();
            Console.WriteLine($"F3={GearList(f)}");
            Assert.AreEqual(22, f.ArmourClass); // Plate + Shield+1 + Defense Fighting Style
            Assert.True(f.ToString().Contains("HP: 28/28"));
        }

        [Test]
        public void TestL4Fighter()
        {
            var f = new Fighter("Test", 4, "Test");
            f.Initialise();
            Console.WriteLine($"F4={GearList(f)}");
            Assert.AreEqual(23, f.ArmourClass); // Plate+1 + Shield+1 + DFS
            Assert.True(f.ToString().Contains("HP: 36/36"));
        }
    }
}