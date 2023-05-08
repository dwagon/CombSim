using System;
using System.Collections.Generic;

namespace CombSim
{
    public abstract class Creature
    {
        public string Name { get; private set; }
        public int HitPoints { get; protected set; }
        public int MaxHitPoints { get; protected set; }
        public string Team { get; protected set; }
        public int ArmourClass { get; protected set; }
        protected readonly Dictionary<StatEnum, Stat> Stats;
        protected string Repr;
        private readonly Conditions _conditions;
        private readonly HashSet<ActionCategory> _actionsThisTurn;
        private List<Equipment> _equipment;
        private readonly List<Action> _actions;
        private readonly int _speed;
        private int _moves;
        public Game game { get; protected set; }

        protected Creature(string name, string team = "")
        {
            Name = name;
            Team = team;
            _speed = 6;
            Stats = new Dictionary<StatEnum, Stat>();
            _conditions = new Conditions();
            _equipment = new List<Equipment>();
            _actions = new List<Action>();
            _actionsThisTurn = new HashSet<ActionCategory>();
        }

        public void Initialise()
        {
            HitPoints = MaxHitPoints;
            _conditions.SetCondition(ConditionEnum.Ok);
        }

        public void SetGame(Game gameGame)
        {
            this.game = gameGame;
        }

        protected void AddEquipment(Equipment gear)
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
        private bool MoveTowards(Location destination)
        {
            if (_moves <= 0)
                return false;
            Location next = game.NextLocationTowards(this, destination);
            game.Move(this, next);
            _moves--;
            return true;
        }
        
        // Move towards a creature
        public bool MoveTowards(Creature creature)
        {
            return MoveTowards(creature.GetLocation());
        }

        public Location GetLocation()
        {
            return game.GetLocation(this);
        }

        public new string ToString()
        {
            return Name + " HP:" + HitPoints + "/" + MaxHitPoints;
        }

        // Return all the possible actions
        private List<IAction> PossibleActions(ActionCategory actionCategory)
        {
            List<IAction> actions = new List<IAction>();
            foreach (var action in _actions)
            {
                if (action.Category == actionCategory)
                {
                    actions.Add(action);
                }
            }
            return actions;
        }

        public void TakeTurn()
        {
            if (!_conditions.HasCondition(ConditionEnum.Ok))
                return;
            StartTurn();
            IAction action = PickActionToDo(ActionCategory.Bonus);
            if (action != null) PerformAction(action);
            action = PickActionToDo(ActionCategory.Action);
            if (action != null) PerformAction(action);
            action = PickActionToDo(ActionCategory.Bonus);
            if (action != null) PerformAction(action);
            EndTurn();
        }
        
        private void EndTurn() {}

        private void PerformAction(IAction action)
        {
            action.DoAction(this);
        }
        
        private IAction PickActionToDo(ActionCategory actionCategory)
        {
            if (!_actionsThisTurn.Contains(actionCategory))
            {
                return null;
            }

            List <IAction> possibleActions = PossibleActions(actionCategory);
            foreach (var action in possibleActions)
            {
                Console.WriteLine($"// {this.Name}: Actions = {action.Name()}");
            }

            if (possibleActions.Count == 0)
                return null;
            Random rnd = new Random();
            int idx = rnd.Next() % possibleActions.Count;
            return possibleActions[idx];  // TODO - make pick best action
        }

        public void TakeDamage(Damage damage)
        {
            HitPoints -= damage.hits;
            NarrationLog.LogMessage($"{Name} took {damage.hits} ({damage.type}) damage");
            if (HitPoints <= 0)
            {
                FallenUnconscious();
            }
        }

        private void FallenUnconscious()
        {
            _conditions.SetCondition(ConditionEnum.Unconscious);
            _conditions.RemoveCondition(ConditionEnum.Ok);
            HitPoints = 0;
        }

        private void StartTurn()
        {
            _moves = _speed;
            _actionsThisTurn.Add(ActionCategory.Action);
            _actionsThisTurn.Add(ActionCategory.Bonus);
            _actionsThisTurn.Add(ActionCategory.Move);
            _actionsThisTurn.Add(ActionCategory.Reaction);
        }
    }
}