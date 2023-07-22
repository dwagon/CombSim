using System;

namespace CombSim.Spells
{
    public class ShockingGrasp : ToHitSpell
    {
        public ShockingGrasp(int casterLevel) : base("Shocking Grasp", 0, ActionCategory.Action)
        {
            Reach = 5 / 5;
            if (casterLevel >= 11) DmgRoll = new DamageRoll("3d8", DamageTypeEnums.Lightning);
            else if (casterLevel >= 5) DmgRoll = new DamageRoll("2d8", DamageTypeEnums.Lightning);
            else DmgRoll = new DamageRoll("1d8", DamageTypeEnums.Lightning);
        }

        protected override void SideEffect(Creature actor, Creature target, Damage damage)
        {
            // Target can't take reactions until the start of its next turn.
            Console.WriteLine("ShockingGrasp Side Effect");
        }
    }
}