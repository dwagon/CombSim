namespace CombSim.Gear
{
    public class Javelin : RangedWeapon
    {
        public Javelin() : base("Javelin", new DamageRoll("1d6", DamageTypeEnums.Piercing), 30 / 5, 120 / 5)
        {
        }
    }

    public static class RangedWeaponGear
    {
        public static readonly RangedWeapon Sling = new RangedWeapon("Sling",
            new DamageRoll("1d4", DamageTypeEnums.Bludgeoning), 30 / 5, 120 / 5);

        public static readonly RangedWeapon Shortbow = new RangedWeapon("Shortbow",
            new DamageRoll("1d6", DamageTypeEnums.Piercing), 80 / 5, 320 / 5);

        public static readonly RangedWeapon ShortbowPlusOne = new RangedWeapon("Shortbow +1",
            new DamageRoll("1d6", DamageTypeEnums.Piercing), 80 / 5, 320 / 5, magicBonus: 1);

        public static readonly RangedWeapon Longbow = new RangedWeapon("Longbow",
            new DamageRoll("1d8", DamageTypeEnums.Piercing), 150 / 5, 600 / 5);

        public static readonly RangedWeapon LongbowPlusOne = new RangedWeapon("Longbow +1",
            new DamageRoll("1d8", DamageTypeEnums.MagicPiercing), 150 / 5, 600 / 5, magicBonus: 1);

        public static readonly RangedWeapon LightCrossbow = new RangedWeapon("LightCrossbow",
            new DamageRoll("1d8", DamageTypeEnums.Piercing), 80 / 5, 320 / 5);
    }
}