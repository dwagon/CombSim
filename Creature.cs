using System;
using System.Collections.Generic;

namespace CombSim
{
    public class Creature
    {
        private readonly List<Action> _actions;
        private readonly HashSet<ActionCategory> _actionsThisTurn;
        private readonly List<Equipment> _equipment;
        private readonly int _speed;
        protected readonly Conditions Conditions;
        public readonly Dictionary<StatEnum, Stat> Stats;
        private int _moves;

        private int _setArmourClass = -1;
        protected int HitPoints;
        protected int MaxHitPoints;
        public EventHandler<OnAttackedEventArgs> OnAttacked;
        public int ProficiencyBonus = 2;
        protected string Repr;

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

        public string Name { get; protected set; }
        public string Team { get; protected set; }

        public int ArmourClass
        {
            get => CalcArmourClass();
            protected set => _setArmourClass = value;
        }

        public Game game { get; private set; }

        public virtual void Initialise()
        {
            HitPoints = MaxHitPoints;
            Conditions.SetCondition(ConditionEnum.Ok);
            OnAttacked += Attacked;
        }

        private int CalcArmourClass()
        {
            if (_setArmourClass >= 0) return _setArmourClass;

            var ac = 10;
            var acBonus = 0;
            var dexBonus = true;
            var maxDexBonus = 99;

            foreach (var gear in _equipment)
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

            var tmpAc = ac + acBonus;
            if (dexBonus)
            {
                var bonus = Stats[StatEnum.Dexterity].Bonus();
                tmpAc += Math.Min(bonus, maxDexBonus);
            }

            if (tmpAc == 0) return 10 + Stats[StatEnum.Dexterity].Bonus();

            return tmpAc;
        }

        private void Attacked(object sender, OnAttackedEventArgs e)
        {
            if (e.CriticalMiss || e.ToHit <= ArmourClass)
            {
                e.AttackMessage.Result = "Miss";
                NarrationLog.LogMessage(e.AttackMessage.ToString());
                return;
            }

            var dmg = e.DmgRoll.Roll(e.CriticalHit);
            e.AttackMessage.Result = $"Hit for {dmg}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
            TakeDamage(dmg);
        }

        public void SetGame(Game gameGame)
        {
            game = gameGame;
        }

        protected void AddEquipment(Equipment gear)
        {
            _equipment.Add(gear);
            foreach (var action in gear.GetActions()) _actions.Add(action);
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
            var next = game.NextLocationTowards(this, destination);
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
            var actions = new List<IAction>();
            foreach (var action in _actions)
                if (action.Category == actionCategory)
                    actions.Add(action);

            return actions;
        }

        public void TakeTurn()
        {
            if (!Conditions.HasCondition(ConditionEnum.Ok))
                return;
            StartTurn();
            var action = PickActionToDo(ActionCategory.Bonus);
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
            if (!_actionsThisTurn.Contains(actionCategory)) return null;

            var possibleActions = PossibleActions(actionCategory);
            foreach (var action in possibleActions) Console.WriteLine($"// {Name}: Actions = {action.Name()}");

            if (possibleActions.Count == 0)
                return null;
            var rnd = new Random();
            var idx = rnd.Next() % possibleActions.Count;
            return possibleActions[idx]; // TODO - make pick best action
        }

        private void TakeDamage(Damage damage)
        {
            HitPoints -= damage.hits;
            if (HitPoints <= 0) FallenUnconscious();
        }

        protected virtual void FallenUnconscious()
        {
        }
        
        // Called when we have died
        protected void Died()
        {
            game.Remove(this);
        }

        private void StartTurn()
        {
            _moves = _speed;
            _actionsThisTurn.Add(ActionCategory.Action);
            _actionsThisTurn.Add(ActionCategory.Bonus);
            _actionsThisTurn.Add(ActionCategory.Move);
            _actionsThisTurn.Add(ActionCategory.Reaction);
        }

        public class OnAttackedEventArgs : EventArgs
        {
            public IAction Action;
            public bool CriticalHit;
            public bool CriticalMiss;
            public DamageRoll DmgRoll;
            public Creature Source;
            public int ToHit;
            public AttackMessage AttackMessage;
        }
    }
}