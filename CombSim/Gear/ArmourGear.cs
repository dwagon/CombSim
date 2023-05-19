namespace CombSim.Gear
{
    public class ArmourGear
    {
        // Light
        public static readonly Armour Leather = new Armour("Leather", 11, dexBonus: true);
        public static readonly Armour StuddedLeather = new Armour("Studded Leather", 12, dexBonus: true);

        // Medium
        public static readonly Armour Hide = new Armour("Hide", 12, dexBonus: true);

        // Heavy
        public static readonly Armour Ring = new Armour("Ring", 14, dexBonus: false);
        public static readonly Armour Plate = new Armour("Plate", 18, dexBonus: false);
        
        // Shields
        public static readonly Armour Shield = new Armour("Shield", armourClassBonus: 2);
    }
}