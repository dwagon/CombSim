using System;

namespace CombSim.Spells
{
    public class ScorchingRay : ToHitSpell
    {
        public ScorchingRay() : base("Scorching Ray", 2, ActionCategory.Action)
        {
            Reach = 120 / 5;
        }

        public override bool DoAction(Creature actor)
        {
            var numMissiles = 3;

            var enemy = actor.PickClosestEnemy();
            var oldLocation = actor.GetLocation();
            if (enemy == null) return false;
            actor.MoveWithinReachOfEnemy(Reach, enemy);

            if (actor.Game.DistanceTo(actor, enemy) <= Reach)
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
            var roll = RollToHit();
            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
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