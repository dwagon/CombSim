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

        private IAction PickActionToDo(ActionCategory actionCategory)
        {
            if (!_actionsThisTurn.Contains(actionCategory)) return null;

            var sortableActions = new List<(int heuristic, IAction action)>();
            var possibleActions = PossibleActions(actionCategory);

            foreach (var action in possibleActions)
            {
                var heuristic = action.GetHeuristic(this);
                Console.WriteLine($"//\tHeuristic of {action.Name()} = {heuristic}");
                if (heuristic > 0) sortableActions.Add((heuristic, action));
            }

            if (!sortableActions.Any()) return null;

            sortableActions.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            return sortableActions.Last().Item2;
        }

        // Return all the possible actions
        private List<IAction> PossibleActions(ActionCategory actionCategory)
        {
            var actions = new List<IAction>();
            foreach (var action in _actions)
                if (action.Category == actionCategory && _actionsThisTurn.Contains(actionCategory))
                    actions.Add(action);

            string actionList = "";
            foreach (var action in actions)
            {
                actionList += action.Name() + "; ";
            }

            Console.WriteLine($"// {Name} Possible {actionCategory} Actions: {actionList}");
            return actions;
        }
    }
}