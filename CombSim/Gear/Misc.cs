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

    public class CloakOfDisplacement : Equipment
    {
        public CloakOfDisplacement() : base("Cloak of Displacement")
        {
        }

        public override bool HasDisadvantageAgainstMe(Creature actor, Creature victim)
        {
            return true;
        }
    }
}