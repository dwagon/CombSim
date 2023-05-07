using System;

namespace CombSim
{
    public class Attack : Action, IAction
    {
        private DamageRoll _dmgRoll;
        protected Attack(string name, DamageRoll damageroll) : base(name, ActionCategory.Action)
        {
            _dmgRoll = damageroll;
        }
        
        public new bool DoAction(Creature actor)
        {
            throw new NotImplementedException();
        }

        private int RollToHit(out bool criticalHit, out bool criticalMiss)
        {
            criticalMiss = false;
            criticalHit = false;
            int roll = Dice.RollD20();
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
        
        protected void DoAttack(Creature target)
        {
            int roll = RollToHit(out bool criticalHit, out bool criticalMiss);

            if (!criticalMiss && roll > target.ArmourClass)
            {
                target.TakeDamage(_dmgRoll.Roll());
            }
        }
    }
}