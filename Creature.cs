using System;
using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public abstract class Creature
    {
        public string Name { get; private set; }
        public int HitPoints { get; protected set; }
        public string Team { get; protected set; }
        protected Dictionary<StatEnum, Stat> Stats;
        protected string _repr;
        private Conditions _conditions;
        private HashSet<ActionCategory> _actionsThisTurn;
        private List<Equipment> _equipment;
        private List<Action> _actions;

        protected Creature(string name, string team = "")
        {
            Name = name;
            Team = team;
            Stats = new Dictionary<StatEnum, Stat>();
            _conditions = new Conditions();
            _equipment = new List<Equipment>();
            _actions = new List<Action>();
            _actionsThisTurn = new HashSet<ActionCategory>();
        }

        public void AddEquipment(Equipment gear)
        {
            _equipment.Add(gear);
            foreach (var action in gear.GetActions())
            {
                _actions.Add(action);
            }
        }

        public string GetRepr()
        {
            return _repr;
        }

        public int RollInitiative()
        {
            return Stats[StatEnum.Dexterity].Roll();
        }

        // Return all the possible actions
        private List<Action> PossibleActions(ActionCategory actcat)
        {
            List<Action> actions = new List<Action>();
            foreach (var action in _actions)
            {
                if (action.Category == actcat)
                {
                    actions.Add(action);
                }
            }
            return actions;
        }

        public void TakeTurn()
        {
            Action action;
            if (_conditions.HasCondition(ConditionEnum.Dead))
                return;
            StartTurn();
            action = PickActionToDo(ActionCategory.Bonus, move: false);
            PerformAction(action);
            action = PickActionToDo(ActionCategory.Action, move: true);
            PerformAction(action);
            action = PickActionToDo(ActionCategory.Bonus, move: false);
            PerformAction(action);
            action = PickActionToDo(ActionCategory.Action, move: true);
            PerformAction(action);
        }

        private void PerformAction(Action action)
        {
            action.DoAction(this);
        }
        
        private Action PickActionToDo(ActionCategory actionCategory, bool move)
        {
            if (!_actionsThisTurn.Contains(actionCategory))
            {
                return null;
            }

            List <Action> possibleActions = PossibleActions(actionCategory);
            foreach (var action in possibleActions)
            {
                Console.WriteLine(this.Name +": Actions = " + action.Name);
            }

            return possibleActions[0];  // TODO - make pick best action
        }

        private void StartTurn()
        {
            _actionsThisTurn.Add(ActionCategory.Action);
            _actionsThisTurn.Add(ActionCategory.Bonus);
            _actionsThisTurn.Add(ActionCategory.Move);
            _actionsThisTurn.Add(ActionCategory.Reaction);

        }
    }
}