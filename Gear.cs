namespace CombSim
{
    public static class Gear
    {
        public static MeleeWeapon Longsword = new MeleeWeapon("Longsword", new DamageRoll("1d8", 0, DamageTypeEnums.Piercing), 5);
        public static MeleeWeapon Dagger = new MeleeWeapon("Dagger", new DamageRoll("1d4", 0, DamageTypeEnums.Piercing), 5);
    }
}