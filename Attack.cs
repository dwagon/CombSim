using System;

namespace CombSim
{
    public class Attack : Action, IAction
    {
        private readonly DamageRoll _dmgRoll;

        protected Attack(string name, DamageRoll damageRoll) : base(name, ActionCategory.Action)
        {
            _dmgRoll = damageRoll;
        }

        public new bool DoAction(Creature actor)
        {
            throw new NotImplementedException();
        }

        private int RollToHit(out bool criticalHit, out bool criticalMiss, bool hasAdvantage = false,
            bool hasDisadvantage = false)
        {
            criticalMiss = false;
            criticalHit = false;
            var roll = Dice.RollD20(hasAdvantage, hasDisadvantage);
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

        protected void DoAttack(Creature actor, Creature target, bool hasAdvantage = false,
            bool hasDisadvantage = false)
        {
            var roll = RollToHit(out var criticalHit, out var criticalMiss, hasAdvantage, hasDisadvantage);
            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
                ToHit = roll,
                DmgRoll = _dmgRoll,
                CriticalHit = criticalHit,
                CriticalMiss = criticalMiss
            });
        }
    }
}