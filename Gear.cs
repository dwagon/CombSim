namespace CombSim
{
    public static class Gear
    {
        public static MeleeWeapon Longsword = new MeleeWeapon("Longsword", new DamageRoll("1d8", 0, DamageTypeEnums.Piercing), 5/5);
        public static MeleeWeapon Dagger = new MeleeWeapon("Dagger", new DamageRoll("1d4", 0, DamageTypeEnums.Piercing), 5/5);
        
        public static RangedWeapon Sling = new RangedWeapon("Sling", new DamageRoll("1d4", 0, DamageTypeEnums.Bludgeoning), 30/5, long_range:120/5);
    }
}