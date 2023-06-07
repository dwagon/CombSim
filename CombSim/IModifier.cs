// For everything that can modify dice rolls, armour class, saving throws etc.

namespace CombSim
{
    public interface IModifier
    {
        int ArmourModification(Attack attack);

        int SavingThrowModification(StatEnum stat);
    }
}