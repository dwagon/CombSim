// Action - creature to do something
//   PerformAction
//      PreAction - tends to be overwritten by Action type (e.g. attack spell)
//      DoAction - will be overwritten by specific action - e.g. fireball
//      PostAction - here for completeness

using System;

namespace CombSim
{
    public enum ActionCategory
    {
        Action,
        Bonus,
        Reaction,
        Supplemental // Things like Action Surge which don't actually take up anything
    }

    public class Action
    {
        private readonly string _name;

        protected Action(string name, ActionCategory category)
        {
            _name = name;
            Category = category;
        }

        public ActionCategory Category { get; set; }

        public virtual int GetHeuristic(Creature actor, out string reason)
        {
            reason = "Default Action";
            return 0;
        }

        public string Name()
        {
            return _name;
        }

        // Wrap the action - should be overwritten by the general action type
        public virtual void PerformAction(Creature actor)
        {
            if (!PreAction(actor))
            {
                return;
            }

            DoAction(actor);
            PostAction(actor);
        }

        protected virtual void DoAction(Creature actor)
        {
            throw new NotImplementedException();
        }

        protected virtual bool PreAction(Creature actor)
        {
            return false;
        }

        protected virtual void PostAction(Creature actor)
        {
        }

        protected static int RollToHit(bool hasAdvantage = false,
            bool hasDisadvantage = false)
        {
            var roll = Dice.RollD20(hasAdvantage, hasDisadvantage, reason: "To Hit");
            return roll;
        }

        protected static bool IsCriticalHit(Creature actor, Creature target, int roll)
        {
            if (target.HasCondition(ConditionEnum.Paralyzed) && actor.DistanceTo(target) < 2)
            {
                return true;
            }

            return roll == 20 || roll >= actor.CriticalHitRoll;
        }

        protected static bool IsCriticalMiss(Creature actor, Creature target, int roll)
        {
            return roll == 1;
        }

        public bool HasAdvantage(Creature actor, Creature target)
        {
            var hasAdvantage = false;
            var hasDisadvantage = false;
            HasAdvantageDisadvantage(actor, target, ref hasAdvantage, ref hasDisadvantage);
            return hasAdvantage;
        }

        public bool HasDisadvantage(Creature actor, Creature target)
        {
            var hasAdvantage = false;
            var hasDisadvantage = false;
            HasAdvantageDisadvantage(actor, target, ref hasAdvantage, ref hasDisadvantage);
            return hasDisadvantage;
        }

        // Overwrite if the attack has a side effect
        protected virtual void SideEffect(Creature actor, Creature target, Damage damage)
        {
        }

        protected void DoHit(Creature actor, Creature target, DamageRoll damageRoll)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());

            target.OnHitAttacked?.Invoke(this, new Creature.OnHitEventArgs()
            {
                Source = actor,
                DmgRoll = damageRoll,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect,
                Attack = this
            });
        }

        // Conditions that cause advantage for the actor against the target
        private static bool HaveAdvantage(Creature actor, Creature target)
        {
            if (target.HasAdvantageAgainstMe(actor)) return true;
            if (actor.DistanceTo(target) < 2)
            {
                if (target.HasCondition(ConditionEnum.Paralyzed)) return true;
                if (target.HasCondition(ConditionEnum.Prone)) return true;
            }

            return false;
        }

        // Conditions that cause disadvantage for the actor against the target
        private static bool HaveDisadvantage(Creature actor, Creature target)
        {
            if (target.HasDisadvantageAgainstMe(actor)) return true;

            if (actor.DistanceTo(target) < 2)
            {
                if (actor.HasCondition(ConditionEnum.Prone)) return true;
            }
            else
            {
                if (target.HasCondition(ConditionEnum.Prone)) return true;
            }

            if (actor.HasCondition(ConditionEnum.Restrained)) return true;
            return false;
        }

        protected static void HasAdvantageDisadvantage(Creature actor, Creature target, ref bool hasAdvantage,
            ref bool hasDisadvantage)
        {
            hasAdvantage = HaveAdvantage(actor, target) || hasAdvantage;
            hasDisadvantage = HaveDisadvantage(actor, target) || hasDisadvantage;

            if (hasAdvantage && hasDisadvantage)
            {
                hasAdvantage = false;
                hasDisadvantage = false;
            }
        }
    }
}