namespace CombSim
{
    public class DcAttack : Attack
    {
        private readonly SpellSavedEffect _saveEffect;
        private readonly int _savingDc;
        private readonly StatEnum _savingStat;
        public readonly int Reach;

        protected DcAttack(string name, StatEnum savingStat, int savingDc, DamageRoll damageRoll,
            SpellSavedEffect saveEffect) : base(name, damageRoll, null)
        {
            Reach = 5 / 5;
            _savingStat = savingStat;
            _savingDc = savingDc;
            _saveEffect = saveEffect;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var enemy = actor.PickClosestEnemy();
            var distance = actor.DistanceTo(enemy);
            if (distance <= Reach)
            {
                reason = $"Enemy {enemy} within range ({distance})";
                return 4;
            }
            else if (distance <= Reach + actor.Speed)
            {
                reason = $"Enemy {enemy} within walking distance ({distance} + {actor.Speed})";
            }

            reason = $"Enemy {enemy} outside range";
            return 0;
        }

        public override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            actor.MoveWithinReachOfCreature(Reach, enemy);

            if (actor.Game.DistanceTo(actor, enemy) <= Reach)
            {
                DoAttack(actor, enemy);
            }
        }

        // Overwrite if the attack has a side effect when you fail the DC
        protected virtual void FailSideEffect(Creature actor, Creature target)
        {
        }

        // Overwrite if the attack has a side effect when you succeed the DC
        protected virtual void SucceedSideEffect(Creature actor, Creature target)
        {
        }

        protected void DoAttack(Creature actor, Creature target)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());

            target.OnDcAttacked?.Invoke(this, new Creature.OnDcAttackedEventArgs()
            {
                Source = actor,
                DcSaveStat = _savingStat,
                DcSaveDc = _savingDc,
                DmgRoll = DamageRoll,
                SpellSavedEffect = _saveEffect,
                AttackMessage = attackMessage,
                OnFailEffect = FailSideEffect,
                OnSucceedEffect = SucceedSideEffect
            });
        }
    }
}