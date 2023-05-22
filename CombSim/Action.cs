using System;

namespace CombSim
{
    public enum ActionCategory
    {
        Action,
        Bonus,
        Move,
        Reaction,
        Supplemental // Things like Action Surge which don't actually take up anything
    }

    public interface IAction
    {
        ActionCategory Category { get; set; }
        void DoAction(Creature actor);
        string Name();
        int GetHeuristic(Creature actor);
    }

    public class Action : IAction
    {
        private readonly string _name;

        protected Action(string name, ActionCategory category)
        {
            _name = name;
            Category = category;
        }

        public ActionCategory Category { get; set; }

        public virtual int GetHeuristic(Creature actor)
        {
            return 1;
        }

        public string Name()
        {
            return _name;
        }

        public virtual void DoAction(Creature actor)
        {
        }

        protected int RollToHit(bool hasAdvantage = false,
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

        protected void HasAdvantageDisadvantage(Creature actor, Creature target, ref bool hasAdvantage,
            ref bool hasDisadvantage)
        {
            Console.WriteLine($"// Before Advantage: {hasAdvantage}\tDisadvantage: {hasDisadvantage}");
            if (actor.DistanceTo(target) < 2)
            {
                if (target.HasCondition(ConditionEnum.Paralyzed)) hasAdvantage = true;
                if (target.HasCondition(ConditionEnum.Prone)) hasAdvantage = true;
                if (actor.HasCondition(ConditionEnum.Prone)) hasDisadvantage = true;
            }
            else
            {
                if (target.HasCondition(ConditionEnum.Prone)) hasDisadvantage = true;
            }

            if (hasAdvantage && hasDisadvantage)
            {
                hasAdvantage = false;
                hasDisadvantage = false;
            }

            Console.WriteLine($"// After Advantage: {hasAdvantage}\tDisadvantage: {hasDisadvantage}");
        }
    }
}