namespace CombSim.Spells
{
    public class ScorchingRay : ToHitSpell
    {
        public ScorchingRay() : base("Scorching Ray", 2, ActionCategory.Action)
        {
            DmgRoll = new DamageRoll("2d6", DamageTypeEnums.Fire);
            Reach = 120 / 5;
        }

        public override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            actor.MoveWithinReachOfEnemy(Reach, enemy);

            if (actor.Game.DistanceTo(actor, enemy) <= Reach)
            {
                for (int i = 0; i < NumberOfMissiles(); i++)
                {
                    if (enemy.HasCondition(ConditionEnum.Ok))
                    {
                        DoMissile(actor, enemy);
                    }
                }

                actor.DoCastSpell(this);
            }
        }

        private int NumberOfMissiles()
        {
            return 3;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            heuristic.AddDamageRoll(DmgRoll);
            heuristic.AddRepeat(NumberOfMissiles());
            return heuristic.GetValue(out reason);
        }

        private void DoMissile(Creature actor, Creature target)
        {
            var hasAdvantage = false;
            var hasDisadvantage = false;

            HasAdvantageDisadvantage(actor, target, ref hasAdvantage, ref hasDisadvantage);
            var roll = RollToHit(hasAdvantage, hasDisadvantage);
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name(),
                roll: roll, mods: actor.SpellAttackModifier());

            target.OnToHitAttacked?.Invoke(this, new Creature.OnToHitAttackedEventArgs()
            {
                Source = actor,
                ToHit = roll + actor.SpellAttackModifier(),
                DmgRoll = DmgRoll,
                CriticalHit = IsCriticalHit(actor, target, roll),
                CriticalMiss = IsCriticalMiss(actor, target, roll),
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}