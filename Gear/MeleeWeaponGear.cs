namespace CombSim.Gear
{
    public class MeleeWeaponGear
    {
        public static readonly MeleeWeapon Longsword =
            new MeleeWeapon("Longsword", new DamageRoll("1d8", 0, DamageTypeEnums.Piercing), 5 / 5);

        public static readonly MeleeWeapon Greatclub =
            new MeleeWeapon("Greatclub", new DamageRoll("1d8", 0, DamageTypeEnums.Bludgeoning), 5 / 5);

        public static readonly MeleeWeapon Dagger =
            new MeleeWeapon("Dagger", new DamageRoll("1d4", 0, DamageTypeEnums.Piercing), 5 / 5);

        public static readonly MeleeWeapon Mace =
            new MeleeWeapon("Mace", new DamageRoll("1d6", 0, DamageTypeEnums.Bludgeoning), 5 / 5);

        public static readonly MeleeWeapon Scimitar =
            new MeleeWeapon("Scimitar", new DamageRoll("1d6", 0, DamageTypeEnums.Slashing), 5 / 5);

        public static readonly MeleeWeapon Shortsword =
            new MeleeWeapon("Shortsword", new DamageRoll("1d4", 0, DamageTypeEnums.Piercing), 5 / 5);

        public static readonly MeleeWeapon Quarterstaff =
            new MeleeWeapon("Quarterstaff", new DamageRoll("1d6", 0, DamageTypeEnums.Bludgeoning), 5 / 5);
    }
}