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

        // Overwrite if the attack has a side effect
        protected virtual void SideEffect(Creature target)
        { }

        protected void DoAttack(Creature actor, Creature target, bool hasAdvantage = false,
            bool hasDisadvantage = false, int attackBonus = 0, int damageBonus = 0)
        {
            if (target.HasCondition(ConditionEnum.Paralyzed) && actor.DistanceTo(target) < 2) hasAdvantage = true;
            
            var roll = RollToHit(hasAdvantage: hasAdvantage,
                hasDisadvantage: hasDisadvantage);
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name(), roll: roll, mods: attackBonus);
            
            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
                ToHit = roll + attackBonus,
                DmgRoll = _dmgRoll + damageBonus,
                CriticalHit = IsCriticalHit(actor, target, roll),
                CriticalMiss = IsCriticalMiss(actor, target, roll),
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}