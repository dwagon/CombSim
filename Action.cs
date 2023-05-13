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
        
        protected int RollToHit(out bool criticalHit, out bool criticalMiss, bool hasAdvantage = false,
            bool hasDisadvantage = false)
        {
            criticalMiss = false;
            criticalHit = false;
            var roll = Dice.RollD20(hasAdvantage, hasDisadvantage, reason: "To Hit");
            switch (roll)
            {
                case 1:
                    criticalMiss = true;
                    break;
                case 20:
                    criticalHit = true;
                    break;
            }

            return roll;
        }
    }
}