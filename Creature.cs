using System;
using System.Collections.Generic;
using System.Linq;

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

        public Game Game { get; private set; }

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
            Game = gameGame;
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
            var next = Game.NextLocationTowards(this, destination);
            Game.Move(this, next);
            _moves--;
            return true;
        }

        // Is this creature able to continue participating in combat
        public bool IsAlive()
        {
            if (Conditions.HasCondition(ConditionEnum.Dead)) return false;
            if (Conditions.HasCondition(ConditionEnum.Unconscious)) return false;
            if (Conditions.HasCondition(ConditionEnum.Stable)) return false;
            return true;
        }

        // Move towards a creature
        public bool MoveTowards(Creature creature)
        {
            return MoveTowards(creature.GetLocation());
        }

        public Location GetLocation()
        {
            return Game.GetLocation(this);
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
            StartTurn();
            if (IsAlive())
            {
                PerformAction(PickActionToDo(ActionCategory.Bonus));
                PerformAction(PickActionToDo(ActionCategory.Action));
                PerformAction(PickActionToDo(ActionCategory.Bonus));
            }

            EndTurn();
        }

        private void EndTurn()
        {
        }

        private void PerformAction(IAction action)
        {
            if (action is null)
            {
                return;
            }
            action.DoAction(this);
            _actionsThisTurn.Remove(action.Category);
        }

        private IAction PickActionToDo(ActionCategory actionCategory)
        {
            var rnd = new Random();

            if (!_actionsThisTurn.Contains(actionCategory)) return null;

            var sortableActions = new List<(int heuristic, IAction action)>();
            var possibleActions = PossibleActions(actionCategory);
            if (possibleActions.Count == 0)
                return null;

            foreach (var action in possibleActions)
            {
                int heuristic = action.GetHeuristic(this) + rnd.Next() % 2;
                sortableActions.Add((heuristic, action));
                Console.WriteLine($"// {Name}: Actions = {action.Name()} {heuristic}");
            }

            sortableActions.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            return sortableActions.Last().Item2;
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
            NarrationLog.LogMessage($"{Name} has died");
            Game.Remove(this);
        }

        protected virtual void StartTurn()
        {
            if(!Conditions.HasCondition(ConditionEnum.Ok)) return;
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