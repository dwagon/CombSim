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
    }
    
    public class Action: IAction
    {
        private string _name;
        public ActionCategory Category { get; private set; }

        public string Name()
        {
            return _name;
        }

        protected Action(string name, ActionCategory category)
        {
            _name = name;
            Category = category;
        }

        public bool DoAction(Creature actor)
        {
            Console.WriteLine("Action.DoAction(" + actor.Name + ") " + this.Name());
            return false;
        }
    }
}