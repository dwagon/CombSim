using System;
using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public class HealingSpell : Spell
    {
        protected DamageRoll HealingRoll;

        protected HealingSpell(string name, int level, ActionCategory actionCategory) : base(name, level,
            actionCategory)
        {
        }

        public override void DoAction(Creature actor)
        {
            if (!actor.CanCastSpell(this)) return;

            var friend = PickClosestFriendNearingHealing(actor, Reach + actor.Speed);
            actor.MoveWithinReachOfCreature(Reach, friend);
            if (actor.Game.DistanceTo(actor, friend) <= Reach)
            {
                actor.DoCastSpell(this);
                DoHealing(actor, friend);
            }
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var friend = PickClosestFriendNearingHealing(actor, Reach + actor.Speed);
            if (friend == null)
            {
                reason = "No one needs healing";
                return 0;
            }

            var hp = HealingRoll.Roll(max: true).hits + actor.HealingBonus();
            hp = Math.Min(hp, friend.HitPointsDown());
            reason = $"Can cure {friend.Name} of {hp}";

            return hp;
        }

        // Pick the friend within range who needs the most healing
        private Creature PickClosestFriendNearingHealing(Creature actor, int range)
        {
            var healingTargets = new List<(Creature, int)>();
            foreach (var friend in actor.GetAllAllies())
            {
                if (friend.PercentHitPoints() < 100 && actor.DistanceTo(friend) <= range)
                {
                    healingTargets.Add((friend, friend.PercentHitPoints()));
                }
            }

            if (healingTargets.Count == 0) return null;
            healingTargets.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            return healingTargets.First().Item1;
        }

        protected virtual void DoHealing(Creature actor, Creature target)
        {
            var hp = HealingRoll.Roll() + actor.HealingBonus();
            target.Heal(hp.hits, Name());
        }
    }
}