namespace CombSim.Gear
{
    public class RangedWeaponGear
    {
        // Ranged Weapons
        public static readonly RangedWeapon Sling = new RangedWeapon("Sling",
            new DamageRoll("1d4", 0, DamageTypeEnums.Bludgeoning), 30 / 5, 120 / 5);

        public static readonly RangedWeapon Shortbow = new RangedWeapon("Shortbow",
            new DamageRoll("1d6", 0, DamageTypeEnums.Piercing), 80 / 5, 320 / 5);
        public static readonly RangedWeapon LightCrossbow = new RangedWeapon("LightCrossbow",
            new DamageRoll("1d8", 0, DamageTypeEnums.Piercing), 80 / 5, 320 / 5);
    }
}