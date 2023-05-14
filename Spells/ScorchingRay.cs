using System;

namespace CombSim.Spells
{
    public class ScorchingRay : ToHitSpell
    {
        public ScorchingRay() : base("Scorching Ray", 2, ActionCategory.Action)
        {
            _reach = 120 / 5;
        }

        public override int GetHeuristic(Creature actor)
        {
            if (actor.CanCastSpell(this)) return 5;
            return 0;
        }

        public override bool DoAction(Creature actor)
        {
            var numMissiles = 3;
            var hasCast = false;

            var enemy = actor.PickClosestEnemy();
            var oldLocation = actor.GetLocation();
            if (enemy == null) return false;
            while (actor.DistanceTo(enemy) > _reach)
                if (!actor.MoveTowards(enemy))
                    break;
            Console.WriteLine($"// {actor.Name} moved from {oldLocation} to {actor.GetLocation()}");

            if (actor.Game.DistanceTo(actor, enemy) <= _reach)
            {
                for (int i = 0; i < numMissiles; i++)
                {
                    DoMissile(actor, enemy);
                }

                actor.DoCastSpell(this);
                return true;
            }

            return false;
        }

        private void DoMissile(Creature actor, Creature target)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());
            var roll = RollToHit(out var criticalHit, out var criticalMiss);
            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
                ToHit = roll + actor.SpellAttackModifier(),
                DmgRoll = new DamageRoll("2d6", DamageTypeEnums.Fire),
                CriticalHit = criticalHit,
                CriticalMiss = criticalMiss,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}