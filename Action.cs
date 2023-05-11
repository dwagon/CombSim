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
    }

    public class Action : IAction
    {
        private readonly string _name;

        protected Action(string name, ActionCategory category)
        {
            _name = name;
            Category = category;
        }

        public ActionCategory Category { get; private set; }

        public int GetHeuristic(Creature actor)
        {
            return 1;
        }

        public string Name()
        {
            return _name;
        }

        public bool DoAction(Creature actor)
        {
            return false;
        }
    }
}