namespace CombSim.Gear
{
    public class RingOfProtection : Equipment
    {
        public RingOfProtection() : base("Ring of Protection")
        {
        }

        public override int ArmourModification()
        {
            return 1;
        }

        public override int SavingThrowModification(StatEnum stat)
        {
            return 1;
        }
    }
}