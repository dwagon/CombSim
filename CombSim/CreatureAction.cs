using System;
using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public partial class Creature
    {
        protected void AddAction(Action action)
        {
            _actions.Add(action);
        }

        public void RemoveAction(Action action)
        {
            _actions.Remove(action);
        }

        private Action PickActionToDo(ActionCategory actionCategory)
        {
            var rnd = new Random();
            if (!_actionsThisTurn.Contains(actionCategory)) return null;

            var sortableActions = new List<(float heuristic, Action action)>();
            var possibleActions = PossibleActions(actionCategory);

            foreach (var action in possibleActions)
            {
                var heuristic = action.GetHeuristic(this, out string reason);
                Console.WriteLine($"//\t{action.Name().PadLeft(25)} = {heuristic}\t{reason}");
                if (heuristic > 0) sortableActions.Add((heuristic + (float)rnd.NextDouble(), action));
            }

            if (!sortableActions.Any()) return null;

            sortableActions.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            return sortableActions.Last().Item2;
        }

        // Return all the possible actions
        private List<Action> PossibleActions(ActionCategory actionCategory)
        {
            var actions = new List<Action>();
            foreach (var action in _actions)
                if (action.Category == actionCategory && _actionsThisTurn.Contains(actionCategory))
                    actions.Add(action);

            string actionList = "";
            foreach (var action in actions)
            {
                actionList += action.Name() + "; ";
            }

            // Console.WriteLine($"// {Name} Possible {actionCategory} Actions: {actionList}");
            return actions;
        }
    }
}