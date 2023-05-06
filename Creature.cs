using System;
using System.Collections.Generic;

namespace CombSim
{
    public abstract class Creature
    {
        public string Name { get; private set; }
        public int HitPoints { get; protected set; }
        public string Team { get; protected set; }
        protected Dictionary<StatEnum, Stat> Stats;
        protected string Repr;
        private Conditions _conditions;
        private HashSet<ActionCategory> _actionsThisTurn;
        private List<Equipment> _equipment;
        private List<Action> _actions;
        private int _speed;
        private int _moves;
        public Game game { get; protected set; }
        public Location Location { get; protected set; }

        protected Creature(string name, string team = "")
        {
            Name = name;
            Team = team;
            _speed = 5;
            Stats = new Dictionary<StatEnum, Stat>();
            _conditions = new Conditions();
            _equipment = new List<Equipment>();
            _actions = new List<Action>();
            _actionsThisTurn = new HashSet<ActionCategory>();
        }

        public void SetGame(Game game)
        {
            this.game = game;
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
            return Repr;
        }

        public int RollInitiative()
        {
            return Stats[StatEnum.Dexterity].Roll();
        }
        
        // Move towards a location
        public bool MoveTowards(Location destination)
        {
            if (_moves <= 0)
                return false;
            Location next = game.NextLocationTowards(Location, destination);
            this.Location = next;
            _moves--;
            return true;
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
            _moves = _speed;
            if (_conditions.HasCondition(ConditionEnum.Dead))
                return;
            StartTurn();
            action = PickActionToDo(ActionCategory.Bonus);
            if (action!=null) PerformAction(action);
            action = PickActionToDo(ActionCategory.Action);
            if (action!=null) PerformAction(action);
            action = PickActionToDo(ActionCategory.Bonus);
            if (action!=null) PerformAction(action);
            EndTurn();
        }
        
        private void EndTurn() {}

        private void PerformAction(Action action)
        {
            Console.WriteLine("PerformAction action=" + action.Name);
            action.DoAction(this);
        }
        
        private Action PickActionToDo(ActionCategory actionCategory)
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

            if (possibleActions.Count == 0)
                return null;
            return possibleActions[0];  // TODO - make pick best action
        }

        private void StartTurn()
        {
            _actionsThisTurn.Add(ActionCategory.Action);
            _actionsThisTurn.Add(ActionCategory.Bonus);
            _actionsThisTurn.Add(ActionCategory.Move);
            _actionsThisTurn.Add(ActionCategory.Reaction);
        }

        public void SetLocation(Location location)
        {
            Location = location;
        }
    }
}