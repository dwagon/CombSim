using System;

namespace CombSim.Spells
{
    public class ShockingGrasp : ToHitSpell
    {
        public ShockingGrasp() : base("Shocking Grasp", 0, ActionCategory.Action)
        {
            Reach = 5 / 5;
            DmgRoll = new DamageRoll("1d8", DamageTypeEnums.Lightning);
        }

        protected override void SideEffect(Creature actor, Creature target)
        {
            // Target can't take reactions until the start of its next turn.
            Console.WriteLine("ShockingGrasp Side Effect");
        }
    }
}