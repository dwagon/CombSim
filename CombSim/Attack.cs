namespace CombSim
{
    public class Attack : Action
    {
        public readonly DamageRoll DamageRoll;
        protected bool Finesse;
        protected bool Versatile;

        protected Attack(string name, DamageRoll damageRoll) : base(name, ActionCategory.Action)
        {
            DamageRoll = damageRoll;
            Finesse = false;
            Versatile = false;
        }

        protected Attack(Weapon weapon) : base(weapon.Name, ActionCategory.Action)
        {
            DamageRoll = weapon.DamageRoll;
            Finesse = weapon.Finesse;
            Versatile = weapon.Versatile;
            Weapon = weapon;
        }

        public Weapon Weapon { get; }

        // Allow attack to proceed by default
        protected override bool PreAction(Creature actor)
        {
            return true;
        }

        // Do all the Side Effects
        private void SideEffects(Creature actor, Creature target, Damage damage)
        {
            if (!target.IsAlive()) return;
            actor.Modifiers.DoAttack(this, actor, target);
            SideEffect(actor, target, damage);
            Weapon?.SideEffect(actor, target);
        }

        protected void DoAttack(Creature actor, Creature target, int attackBonus = 0, int damageBonus = 0)
        {
            var hasDisadvantage = false;
            var hasAdvantage = false;
            HasAdvantageDisadvantage(actor, target, ref hasAdvantage, ref hasDisadvantage);

            if (Weapon != null)
            {
                attackBonus += Weapon.MagicBonus;
                damageBonus += Weapon.MagicBonus;
            }

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