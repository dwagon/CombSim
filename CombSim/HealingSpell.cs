using System;
using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public class HealingSpell : Spell
    {
        protected DamageRoll HealingRoll;
        protected int NumRecipients = 1;

        protected HealingSpell(string name, int level, ActionCategory actionCategory) : base(name, level,
            actionCategory)
        {
        }

        protected override void DoAction(Creature actor)
        {
            bool hasCast = false;

            var friends = PickClosestFriendsNeedingHealing(actor, Reach + actor.Speed);
            var sickestFriend = friends.First();
            actor.MoveWithinReachOfCreature(Reach, sickestFriend);
            foreach (var friend in friends)
            {
                if (actor.Game.DistanceTo(actor, sickestFriend) <= Reach)
                {
                    if (!hasCast)
                    {
                        actor.DoCastSpell(this);
                        hasCast = true;
                    }

                    DoHealing(actor, friend);
                }
            }
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            if (!actor.CanCastSpell(this))
            {
                reason = $"{actor.Name} can't cast {Name()}";
                return 0;
            }

            var friends = PickClosestFriendsNeedingHealing(actor, Reach + actor.Speed);
            if (friends == null)
            {
                reason = "No one needs healing";
                return 0;
            }

            var totalCure = 0;
            foreach (var critter in friends)
            {
                var hp = HealingAmount(actor, critter, true);
                totalCure += Math.Min(hp.hits, critter.HitPointsDown());
            }

            reason = $"Can cure {friends.Count} friends of {totalCure}";

            return totalCure;
        }

        // Pick the {numFriends} friends within range who needs the most healing
        private List<Creature> PickClosestFriendsNeedingHealing(Creature actor, int range)
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
            var returnList = new List<Creature>();
            var numFriends = Math.Min(NumRecipients, healingTargets.Count);
            foreach (var target in healingTargets.GetRange(0, numFriends))
            {
                returnList.Add(target.Item1);
            }

            return returnList;
        }

        // How much do we heal with this spell
        protected virtual Damage HealingAmount(Creature actor, Creature target, bool max = false)
        {
            var hp = HealingRoll.Roll(max: max) + actor.HealingBonus();
            return hp;
        }

        protected virtual void DoHealing(Creature actor, Creature target)
        {
            var hp = HealingAmount(actor, target);
            target.Heal(hp.hits, Name());
        }
    }
}