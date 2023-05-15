namespace CombSim.Gear
{
    public class ArmourGear
    {
        // Armour
        public static readonly Armour Ring = new Armour("Ring", 14, dexBonus: false);
        public static readonly Armour Plate = new Armour("Plate", 18, dexBonus: false);
        public static readonly Armour Leather = new Armour("Leather", 11, dexBonus: true);
        public static readonly Armour Shield = new Armour("Shield", armourClassBonus: 2);
    }
}