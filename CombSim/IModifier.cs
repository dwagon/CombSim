// For everything that can modify dice rolls, armour class, saving throws etc.

namespace CombSim
{
    public interface IModifier
    {
        // Numeric change to AC based on the attack
        int ArmourModification(Attack attack);

        // Numeric modification to a saving throw
        int SavingThrowModification(StatEnum stat);

        // Does {actor} have disadvantage against {victim}
        bool HasDisadvantageAgainstMe(Creature actor, Creature victim);

        // Does {actor} have disadvantage against {victim}
        bool HasAdvantageAgainstMe(Creature actor, Creature victim);

        // {actor} to perform an {attack} against {victim}
        void DoAttack(Attack attackAction, Creature actor, Creature victim);
    }
}