using System;

namespace CombSim
{
    public class Attack : Action
    {
        public readonly DamageRoll DamageRoll;
        protected bool Finesse;
        protected bool Versatile;

        protected Attack(string name, DamageRoll damageRoll, Weapon weapon) : base(name, ActionCategory.Action)
        {
            DamageRoll = damageRoll;
            Finesse = false;
            Versatile = false;
            Weapon = weapon;
        }

        public Weapon Weapon { get; }

        public override void DoAction(Creature actor)
        {
            throw new NotImplementedException();
        }

        // Overwrite if the attack has a side effect
        protected virtual void SideEffect(Creature actor, Creature target)
        {
        }

        // Do all the Side Effects
        protected void SideEffects(Creature actor, Creature target)
        {
            actor.Effects.DoAttack(this, actor, target);
            SideEffect(actor, target);
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
                DmgRoll = DamageRoll + damageBonus,
                CriticalHit = IsCriticalHit(actor, target, roll),
                CriticalMiss = IsCriticalMiss(actor, target, roll),
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffects,
                Attack = this
            });
        }
    }
}