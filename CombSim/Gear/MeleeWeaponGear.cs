namespace CombSim.Gear
{
    public static class MeleeWeaponGear
    {
        public static readonly MeleeWeapon Longsword =
            new MeleeWeapon("Longsword", new DamageRoll("1d8", DamageTypeEnums.Piercing));

        public static readonly MeleeWeapon Greatclub =
            new MeleeWeapon("Greatclub", new DamageRoll("1d8", DamageTypeEnums.Bludgeoning));

        public static readonly MeleeWeapon Dagger =
            new MeleeWeapon("Dagger", new DamageRoll("1d4", DamageTypeEnums.Piercing), finesse: true);

        public static readonly MeleeWeapon Mace =
            new MeleeWeapon("Mace", new DamageRoll("1d6", DamageTypeEnums.Bludgeoning));

        public static readonly MeleeWeapon MacePlusOne =
            new MeleeWeapon("Mace +1", new DamageRoll("1d6", DamageTypeEnums.MagicBludgeoning), magicBonus: 1);

        public static readonly MeleeWeapon Flail =
            new MeleeWeapon("Flail", new DamageRoll("1d8", DamageTypeEnums.Bludgeoning));

        public static readonly MeleeWeapon Scimitar =
            new MeleeWeapon("Scimitar", new DamageRoll("1d6", DamageTypeEnums.Slashing), finesse: true);

        public static readonly MeleeWeapon Shortsword =
            new MeleeWeapon("Shortsword", new DamageRoll("1d6", DamageTypeEnums.Piercing));

        public static readonly MeleeWeapon Quarterstaff =
            new MeleeWeapon("Quarterstaff", new DamageRoll("1d6", DamageTypeEnums.Bludgeoning));

        public static readonly MeleeWeapon Spear =
            new MeleeWeapon("Spear", new DamageRoll("1d6", DamageTypeEnums.Piercing));

        public static readonly MeleeWeapon Morningstar =
            new MeleeWeapon("Morningstar", new DamageRoll("1d8", DamageTypeEnums.Piercing));
    }
}