namespace CombSim.Gear
{
    public class RingOfProtection : Equipment
    {
        public RingOfProtection() : base("Ring of Protection")
        {
        }

        public override int ArmourModification(Attack attack)
        {
            return 1;
        }

        public override int SavingThrowModification(StatEnum stat)
        {
            return 1;
        }
    }

    public class BracersOfDefence : Armour
    {
        public BracersOfDefence() : base("Bracers of Defence", armourClassBonus: 2)
        {
        }
    }
}