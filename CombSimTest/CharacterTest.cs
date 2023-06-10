using CombSim.Characters;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class CharacterTest
    {
        [Test]
        public void TestProficiency()
        {
            var l1 = new Fighter("L1", 1);
            Assert.AreEqual(2, l1.ProficiencyBonus);
            var l2 = new Fighter("L2", 2);
            Assert.AreEqual(2, l2.ProficiencyBonus);

            var l5 = new Fighter("L5", 5);
            Assert.AreEqual(3, l5.ProficiencyBonus);
        }
    }
}