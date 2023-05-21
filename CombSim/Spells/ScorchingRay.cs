namespace CombSim.Spells
{
    public class ScorchingRay : ToHitSpell
    {
        public ScorchingRay() : base("Scorching Ray", 2, ActionCategory.Action)
        {
            Reach = 120 / 5;
        }

        public override void DoAction(Creature actor)
        {
            var numMissiles = 3;

            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            actor.MoveWithinReachOfEnemy(Reach, enemy);

            if (actor.Game.DistanceTo(actor, enemy) <= Reach)
            {
                for (int i = 0; i < numMissiles; i++)
                {
                    DoMissile(actor, enemy);
                }

                actor.DoCastSpell(this);
            }
        }

        private void DoMissile(Creature actor, Creature target)
        {
            var roll = RollToHit();
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name(),
                roll: roll, mods: actor.SpellAttackModifier());

            target.OnToHitAttacked?.Invoke(this, new Creature.OnToHitAttackedEventArgs()
            {
                Source = actor,
                ToHit = roll + actor.SpellAttackModifier(),
                DmgRoll = new DamageRoll("2d6", DamageTypeEnums.Fire),
                CriticalHit = IsCriticalHit(actor, target, roll),
                CriticalMiss = IsCriticalMiss(actor, target, roll),
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}