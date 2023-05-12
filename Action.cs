using System;

namespace CombSim
{
    public enum ActionCategory
    {
        Action,
        Bonus,
        Move,
        Reaction
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
    }
}