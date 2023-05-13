namespace CombSim
{
    public static class Gear
    {
        // Melee Weapons
        public static readonly MeleeWeapon Longsword =
            new MeleeWeapon("Longsword", new DamageRoll("1d8", 0, DamageTypeEnums.Piercing), 5 / 5);

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

        // Ranged Weapons
        public static readonly RangedWeapon Sling = new RangedWeapon("Sling",
            new DamageRoll("1d4", 0, DamageTypeEnums.Bludgeoning), 30 / 5, 120 / 5);

        public static readonly RangedWeapon Shortbow = new RangedWeapon("Shortbow",
            new DamageRoll("1d6", 0, DamageTypeEnums.Piercing), 80 / 5, 320 / 5);
        public static readonly RangedWeapon LightCrossbow = new RangedWeapon("LightCrossbow",
            new DamageRoll("1d8", 0, DamageTypeEnums.Piercing), 80 / 5, 320 / 5);

        // Armour
        public static readonly Armour Ring = new Armour("Ring", 14, dexBonus: false);
        public static readonly Armour Plate = new Armour("Plate", 18, dexBonus: false);
        public static readonly Armour Leather = new Armour("Leather", 11, dexBonus: true);
        public static readonly Armour Shield = new Armour("Shield", armourClassBonus: 2);
    }
}