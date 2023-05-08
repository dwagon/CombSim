using System;

namespace CombSim
{
    public static class Gear
    {
        public static readonly MeleeWeapon Longsword = new MeleeWeapon("Longsword", new DamageRoll("1d8", 0, DamageTypeEnums.Piercing), 5/5);
        public static readonly MeleeWeapon Dagger = new MeleeWeapon("Dagger", new DamageRoll("1d4", 0, DamageTypeEnums.Piercing), 5/5);
        
        public static readonly RangedWeapon Sling = new RangedWeapon("Sling", new DamageRoll("1d4", 0, DamageTypeEnums.Bludgeoning), 30/5, long_range:120/5);

        public static readonly Armour Plate = new Armour("Plate", armourClass:18, dexBonus: false);
        public static readonly Armour Shield = new Armour("Shield", armourClassBonus:2);
    }
}