using CombSim;
using NUnit.Framework;

namespace CombSimTest
{
    [TestFixture]
    public class EquipmentTest
    {
        [Test]
        public void TestRangedWeapon()
        {
            var ammo = 2;
            var rw = new RangedWeapon("TestWeapon", new DamageRoll("1d6", DamageTypeEnums.Psychic), 10, 20, ammo);
            Assert.True(rw.HasAmmunition());
            rw.UseWeapon();
            Assert.AreEqual(1, rw.GetAmmunition());
            rw.UseWeapon();
            Assert.False(rw.HasAmmunition());
        }
    }
}