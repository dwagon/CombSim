using System;
using System.Collections.Generic;

namespace CombSim
{
    public class Creature
    {
        public string Name { get; protected set; }
        protected int HitPoints;
        protected int MaxHitPoints;
        public string Team { get; protected set; }

        public int ArmourClass
        {
            get => CalcArmourClass();
            protected set => _setArmourClass = value;
        }

        private int _setArmourClass = -1;
        protected readonly Dictionary<StatEnum, Stat> Stats;
        protected string Repr;
        protected readonly Conditions Conditions;
        private readonly HashSet<ActionCategory> _actionsThisTurn;
        private readonly List<Equipment> _equipment;
        private readonly List<Action> _actions;
        private readonly int _speed;
        private int _moves;
        public Game game { get; private set; }
        public EventHandler<OnAttackedEventArgs> OnAttacked;

        public class OnAttackedEventArgs : EventArgs
        {
            public Creature Source;
            public IAction Action;
            public int ToHit;
            public DamageRoll DmgRoll;
            public bool CriticalHit;
            public bool CriticalMiss;
        }

        protected Creature(string name, string team = "")
        {
            Name = name;
            Team = team;
            _speed = 6;
            Stats = new Dictionary<StatEnum, Stat>();
            Conditions = new Conditions();
            _equipment = new List<Equipment>();
            _actions = new List<Action>();
            _actionsThisTurn = new HashSet<ActionCategory>();
        }

        public virtual void Initialise()
        {
            HitPoints = MaxHitPoints;
            Conditions.SetCondition(ConditionEnum.Ok);
            OnAttacked += Attacked;
        }

        private int CalcArmourClass()
        {
            if (_setArmourClass >= 0)
            {
                return _setArmourClass;
            }

            var ac = 10;
            var acBonus = 0;
            var dexBonus = true;
            var maxDexBonus = 99;

            foreach (var gear in _equipment)
            {
                if (gear is Armour armour)
                {
                    if (armour.ArmourClass > 0)
                    {
                        if (!armour.DexBonus) dexBonus = false;
                        if (armour.MaxDexBonus >= 0) maxDexBonus = Math.Min(maxDexBonus, armour.MaxDexBonus);
                        ac = Math.Max(armour.ArmourClass, ac);
                    }

                    acBonus += Math.Max(acBonus, armour.ArmourClassBonus);
                }
            }

            int tmpAc = ac + acBonus;
            if (dexBonus)
            {
                var bonus = Stats[StatEnum.Dexterity].Bonus();
                tmpAc += Math.Min(bonus, maxDexBonus);
            }

            if (tmpAc == 0)
            {
                return 10 + Stats[StatEnum.Dexterity].Bonus();
            }

            return tmpAc;
        }

        private void Attacked(object sender, OnAttackedEventArgs e)
        {
            if (e.CriticalMiss || e.ToHit <= ArmourClass)
            {
                NarrationLog.LogMessage(
                    $"{e.Source.Name} missed {Name} with {e.Action.Name()} (Rolled {e.ToHit} vs AC {ArmourClass})");
                return;
            }

            var dmg = e.DmgRoll.Roll(max: e.CriticalHit);
            NarrationLog.LogMessage($"{e.Source.Name} hit {Name} with {e.Action.Name()} (Rolled {e.ToHit}) for {dmg}");
            TakeDamage(dmg);
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

        public bool IsAlive()
        {
            if (Conditions.HasCondition(ConditionEnum.Dead)) return false;
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
            return $"{Name} HP: {HitPoints}/{MaxHitPoints} {Conditions}";
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
            if (!Conditions.HasCondition(ConditionEnum.Ok))
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

        private void EndTurn()
        {
        }

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

            List<IAction> possibleActions = PossibleActions(actionCategory);
            foreach (var action in possibleActions)
            {
                Console.WriteLine($"// {this.Name}: Actions = {action.Name()}");
            }

            if (possibleActions.Count == 0)
                return null;
            Random rnd = new Random();
            int idx = rnd.Next() % possibleActions.Count;
            return possibleActions[idx]; // TODO - make pick best action
        }

        private void TakeDamage(Damage damage)
        {
            HitPoints -= damage.hits;
            if (HitPoints <= 0) FallenUnconscious();
            NarrationLog.LogMessage($"{Name} took {damage.hits} {damage.type} damage. Now has {HitPoints}");
        }

        protected virtual void FallenUnconscious()
        {
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