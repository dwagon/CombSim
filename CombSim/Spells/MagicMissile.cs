namespace CombSim.Spells
{
    public class MagicMissile : Spell
    {
        private DamageRoll _damageRoll = new DamageRoll("1d4+1", DamageTypeEnums.Force);

        public MagicMissile() : base("Magic Missile", 1, ActionCategory.Action)
        {
            Reach = 120 / 5;
        }

        private int NumberOfMissiles()
        {
            return 3;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            heuristic.AddDamageRoll(_damageRoll);
            heuristic.AddRepeat(NumberOfMissiles());
            return heuristic.GetValue(out reason);
        }

        public override void DoAction(Creature actor)
        {
            var numMissiles = NumberOfMissiles();
            var hasCast = false;
            for (int i = 0; i < numMissiles; i++)
            {
                var enemy = actor.PickClosestEnemy();
                if (enemy == null) return;
                actor.MoveWithinReachOfEnemy(Reach, enemy);

                if (actor.Game.DistanceTo(actor, enemy) <= Reach)
                {
                    hasCast = true;
                    DoMissile(actor, enemy);
                }
            }

            if (hasCast)
                actor.DoCastSpell(this);
        }

        private void DoMissile(Creature actor, Creature target)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());

            target.OnToHitAttacked?.Invoke(this, new Creature.OnToHitAttackedEventArgs()
            {
                Source = actor,
                ToHit = 999, // Autohit
                DmgRoll = _damageRoll,
                CriticalHit = false,
                CriticalMiss = false,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}