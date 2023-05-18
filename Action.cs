namespace CombSim
{
    public enum ActionCategory
    {
        Action,
        Bonus,
        Move,
        Reaction,
        Supplemental    // Things like Action Surge which don't actually take up anything
    }

    public interface IAction
    {
        bool DoAction(Creature actor);
        string Name();
        int GetHeuristic(Creature actor);
        ActionCategory Category { get; set; }
    }

    public class Action : IAction
    {
        private readonly string _name;
        public ActionCategory Category { get; set; }

        protected Action(string name, ActionCategory category)
        {
            _name = name;
            Category = category;
        }

        public virtual int GetHeuristic(Creature actor)
        {
            return 1;
        }

        public string Name()
        {
            return _name;
        }

        public virtual bool DoAction(Creature actor)
        {
            return false;
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
    }
}