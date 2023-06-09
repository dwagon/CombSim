using System;

namespace CombSim.Gear
{
    public static class ArmourGear
    {
        // Light
        public static readonly Armour Leather = new Armour("Leather", 11, dexBonus: true);
        public static readonly Armour LeatherPlusOne = new Armour("Leather +1", 11, dexBonus: true, magicBonus: 1);
        public static readonly Armour StuddedLeather = new Armour("Studded Leather", 12, dexBonus: true);

        public static readonly Armour StuddedLeatherPlusOne =
            new Armour("Studded Leather +1", 12, dexBonus: true, magicBonus: 1);

        // Medium
        public static readonly Armour Hide = new Armour("Hide", 12, dexBonus: true);

        // Heavy
        public static readonly Armour Ring = new Armour("Ring", 14, dexBonus: false);
        public static readonly Armour Plate = new Armour("Plate", 18, dexBonus: false);
        public static readonly Armour PlatePlusOne = new Armour("Plate +1", 18, dexBonus: false, magicBonus: 1);
        public static readonly Armour PlatePlusTwo = new Armour("Plate +1", 18, dexBonus: false, magicBonus: 2);

        // Shields
        public static readonly Armour Shield = new Armour("Shield", armourClassBonus: 2);
        public static readonly Armour ShieldPlusOne = new Armour("Shield +1", armourClassBonus: 2, magicBonus: 1);

        public class ArrowCatchingShield : Armour
        {
            public ArrowCatchingShield() : base("Arrow Catching Shield", armourClassBonus: 2)
            {
            }

            // You gain a +2 bonus to AC against ranged attacks while you wield this shield. 
            public override int ArmourModification(Attack attack)
            {
                if (attack is RangedAttack)
                {
                    Console.WriteLine("Arrow Catching Shield caught an arrow");
                    return 2;
                }

                return 0;
            }
        }
    }
}