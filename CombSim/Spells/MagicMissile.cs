namespace CombSim.Spells
{
    public class MagicMissile : Spell
    {
        public MagicMissile() : base("Magic Missile", 1, ActionCategory.Action)
        {
            Reach = 120 / 5;
        }

        public override void DoAction(Creature actor)
        {
            var numMissiles = 3;
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
                DmgRoll = new DamageRoll("1d4+1", DamageTypeEnums.Force),
                CriticalHit = false,
                CriticalMiss = false,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}