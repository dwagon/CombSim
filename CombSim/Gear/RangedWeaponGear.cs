namespace CombSim.Gear
{
    public class RangedWeaponGear
    {
        public static readonly RangedWeapon Sling = new RangedWeapon("Sling",
            new DamageRoll("1d4", DamageTypeEnums.Bludgeoning), 30 / 5, 120 / 5);

        public static readonly RangedWeapon Javelin = new RangedWeapon("Javelin",
            new DamageRoll("1d6", DamageTypeEnums.Piercing), 30 / 5, 120 / 5);

        public static readonly RangedWeapon Shortbow = new RangedWeapon("Shortbow",
            new DamageRoll("1d6", DamageTypeEnums.Piercing), 80 / 5, 320 / 5);

        public static readonly RangedWeapon Longbow = new RangedWeapon("Longbow",
            new DamageRoll("1d8", DamageTypeEnums.Piercing), 150 / 5, 600 / 5);

        public static readonly RangedWeapon LightCrossbow = new RangedWeapon("LightCrossbow",
            new DamageRoll("1d8", DamageTypeEnums.Piercing), 80 / 5, 320 / 5);
    }
}