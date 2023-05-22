namespace CombSim
{
    public class DcAttack : Attack
    {
        private readonly int _reach;
        private readonly SpellSavedEffect _saveEffect;
        private readonly int _savingDc;
        private readonly StatEnum _savingStat;

        public DcAttack(string name, StatEnum savingStat, int savingDc, DamageRoll damageRoll,
            SpellSavedEffect saveEffect) : base(name, damageRoll)
        {
            _reach = 1;
            _savingStat = savingStat;
            _savingDc = savingDc;
            _saveEffect = saveEffect;
        }

        public override int GetHeuristic(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return 0;
            if (actor.DistanceTo(enemy) <= 2) return 4;
            return 1;
        }

        public override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            actor.MoveWithinReachOfEnemy(_reach, enemy);

            if (actor.Game.DistanceTo(actor, enemy) <= _reach)
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
                DmgRoll = _dmgRoll,
                SpellSavedEffect = _saveEffect,
                AttackMessage = attackMessage,
                OnFailEffect = FailSideEffect,
                OnSucceedEffect = SucceedSideEffect
            });
        }
    }
}