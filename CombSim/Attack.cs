using System;

namespace CombSim
{
    public class Attack : Action
    {
        protected readonly DamageRoll _dmgRoll;
        protected bool Versatile;

        protected Attack(string name, DamageRoll damageRoll) : base(name, ActionCategory.Action)
        {
            _dmgRoll = damageRoll;
            Versatile = false;
        }

        public override void DoAction(Creature actor)
        {
            throw new NotImplementedException();
        }

        // Overwrite if the attack has a side effect
        protected virtual void SideEffect(Creature actor, Creature target)
        {
        }

        protected void DoAttack(Creature actor, Creature target, int attackBonus = 0, int damageBonus = 0)
        {
            var hasDisadvantage = false;
            var hasAdvantage = false;
            HasAdvantageDisadvantage(actor, target, ref hasAdvantage, ref hasDisadvantage);

            var roll = RollToHit(hasAdvantage: hasAdvantage, hasDisadvantage: hasDisadvantage);
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name(),
                roll: roll, mods: attackBonus);

            target.OnToHitAttacked?.Invoke(this, new Creature.OnToHitAttackedEventArgs
            {
                Source = actor,
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