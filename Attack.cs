using System;

namespace CombSim
{
    public class Attack : Action, IAction
    {
        private DamageRoll _dmgRoll;
        protected Attack(string name, DamageRoll damageRoll) : base(name, ActionCategory.Action)
        {
            _dmgRoll = damageRoll;
        }
        
        public new bool DoAction(Creature actor)
        {
            throw new NotImplementedException();
        }

        private int RollToHit(out bool criticalHit, out bool criticalMiss, bool hasAdvantage = false, bool hasDisadvantage = false)
        {
            criticalMiss = false;
            criticalHit = false;
            int roll = Dice.RollD20(hasAdvantage, hasDisadvantage);
            switch (roll) 
            {
                case 1: 
                    criticalMiss = true;
                    break;
                case 20: 
                    criticalHit = true;
                    break;
            }
            return roll;
        }
        
        protected void DoAttack(Creature target, bool hasAdvantage=false, bool hasDisadvantage=false)
        {
            int roll = RollToHit(out bool criticalHit, out bool criticalMiss, hasAdvantage, hasDisadvantage);

            if (!criticalMiss && roll > target.ArmourClass)
            {
                target.TakeDamage(_dmgRoll.Roll());
            }
        }
    }
}