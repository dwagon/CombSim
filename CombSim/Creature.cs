using System;
using System.Collections.Generic;

namespace CombSim
{
    public partial class Creature
    {
        private readonly List<Action> _actions;
        private readonly HashSet<ActionCategory> _actionsThisTurn;
        private readonly List<Equipment> _equipment;
        private readonly int _speed;
        private readonly Dictionary<string, Spell> _spells;
        protected readonly Conditions Conditions;
        protected readonly List<Damage> DamageReceived;
        private readonly Effects Effects;
        protected readonly List<DamageTypeEnums> Immune;
        protected readonly List<DamageTypeEnums> Resistant;
        public readonly Dictionary<StatEnum, Stat> Stats;
        protected readonly List<DamageTypeEnums> Vulnerable;
        private int _moves;

        private int _setArmourClass = -1;
        protected int HitPoints;
        protected int MaxHitPoints;
        public EventHandler<OnTurnEndEventArgs> OnTurnEnd;
        public EventHandler<OnTurnStartEventArgs> OnTurnStart;
        public int ProficiencyBonus = 2;
        protected string Repr;
        protected StatEnum SpellCastingAbility;

        protected Creature(string name, string team = "")
        {
            Name = name;
            Team = team;
            _speed = 6;
            Stats = new Dictionary<StatEnum, Stat>();
            Conditions = new Conditions();
            Effects = new Effects();
            _equipment = new List<Equipment>();
            _actions = new List<Action>();
            _actionsThisTurn = new HashSet<ActionCategory>();
            _spells = new Dictionary<string, Spell>();
            Vulnerable = new List<DamageTypeEnums>();
            Resistant = new List<DamageTypeEnums>();
            Immune = new List<DamageTypeEnums>();
            DamageReceived = new List<Damage>();
            CriticalHitRoll = 20;
            Attributes = new HashSet<Attribute>();
        }

        protected HashSet<Attribute> Attributes { get; private set; }
        public int CriticalHitRoll { get; protected set; }

        public string Name { get; protected set; }
        public string Team { get; protected set; }

        public int ArmourClass
        {
            get => CalcArmourClass();
            protected set => _setArmourClass = value;
        }

        public Game Game { get; private set; }

        public bool HasAttribute(Attribute attribute)
        {
            return Attributes.Contains(attribute);
        }

        public int HitPointsDown()
        {
            return MaxHitPoints - HitPoints;
        }

        // What percentage of maximum hit points does the creature have
        public int PercentHitPoints()
        {
            return (int)(100 * ((float)HitPoints / MaxHitPoints));
        }

        public virtual void Initialise()
        {
            HitPoints = MaxHitPoints;
            Conditions.SetCondition(ConditionEnum.Ok);
            BeingAttackedInitialise();
        }

        public List<Location> GetNeighbourLocations()
        {
            return Game.GetNeighbourLocations(this);
        }

        public List<Creature> GetNeighbourCreatures()
        {
            var creatures = new List<Creature>();
            foreach (var location in GetNeighbourLocations())
            {
                var critter = Game.GetCreatureAtLocation(location);
                if (critter != null)
                {
                    creatures.Add(critter);
                }
            }

            return creatures;
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

            var tmpAc = ac + acBonus + GetDexAcBonus(dexBonus, maxDexBonus);

            if (tmpAc == 0) return 10 + Stats[StatEnum.Dexterity].Bonus();

            return tmpAc;
        }

        private int GetDexAcBonus(bool dexBonus, int maxDexBonus)
        {
            if (!dexBonus) return 0;
            return Math.Min(Stats[StatEnum.Dexterity].Bonus(), maxDexBonus);
        }

        public bool HasCondition(ConditionEnum condition)
        {
            return Conditions.HasCondition(condition);
        }

        public void AddCondition(ConditionEnum condition)
        {
            Conditions.SetCondition(condition);
        }

        public void RemoveCondition(ConditionEnum condition)
        {
            Conditions.RemoveCondition(condition);
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

        public new virtual string ToString()
        {
            return $"{Name} AC: {ArmourClass}; HP: {HitPoints}/{MaxHitPoints}; {Conditions}; {Effects}";
        }

        public int Heal(int hitPoints, string reason = "")
        {
            string reasonString = "";
            hitPoints = Math.Min(hitPoints, HitPointsDown());
            HitPoints += hitPoints;
            if (reason != "")
            {
                reasonString = $" by {reason}";
            }

            NarrationLog.LogMessage($"{Name} healed {hitPoints}{reasonString}");

            if (Conditions.HasCondition(ConditionEnum.Stable))
            {
                Conditions.RemoveCondition(ConditionEnum.Stable);
                Conditions.SetCondition(ConditionEnum.Ok);
            }

            if (Conditions.HasCondition(ConditionEnum.Unconscious))
            {
                Conditions.RemoveCondition(ConditionEnum.Unconscious);
                Conditions.SetCondition(ConditionEnum.Ok);
            }

            return hitPoints;
        }

        public float DistanceTo(Creature enemy)
        {
            return Game.DistanceTo(this, enemy);
        }

        public IEnumerable<Location> GetConeLocations(int coneSize, GridDirection direction)
        {
            return Game.GetConeLocations(this, coneSize, direction);
        }

        public void DoActionCategory(ActionCategory actionCategory, bool force = false)
        {
            if (force) _actionsThisTurn.Add(actionCategory);
            PerformAction(PickActionToDo(actionCategory));
        }

        public void TakeTurn()
        {
            Console.WriteLine($"// {Name} -----------------------------------------------");
            TurnStart();
            if (IsAlive())
            {
                DoActionCategory(ActionCategory.Action);
                DoActionCategory(ActionCategory.Supplemental);
                DoActionCategory(ActionCategory.Bonus);
            }

            TurnEnd();
        }

        private void TurnEnd()
        {
            OnTurnEnd?.Invoke(this, new OnTurnEndEventArgs
            {
                Creature = this
            });
        }

        private void PerformAction(IAction action)
        {
            if (action is null) return;

            Console.WriteLine($"// {Name} doing {action.Name()}");
            action.DoAction(this);
            _actionsThisTurn.Remove(action.Category);
        }

        protected virtual void FallenUnconscious()
        {
        }

        public bool MakeSavingThrow(StatEnum stat, int dc)
        {
            Console.Write($"// {Name} does {stat} saving vs DC {dc} - ");
            if (HasCondition(ConditionEnum.Paralyzed))
            {
                if (stat == StatEnum.Strength || stat == StatEnum.Dexterity) return false;
            }

            var roll = Stats[stat].Roll();
            if (roll > dc)
            {
                Console.WriteLine($"{roll} Success");
                return true;
            }

            Console.WriteLine($"{roll} Failure");
            return false;
        }

        // Called when we have died
        protected void Died()
        {
            if (!HasCondition(ConditionEnum.Dead)) NarrationLog.LogMessage($"{Name} has died");
            Conditions.RemoveAllConditions();
            Conditions.SetCondition(ConditionEnum.Dead);
            Game.Remove(this);
        }

        protected virtual void TurnStart()
        {
            OnTurnStart?.Invoke(this, new OnTurnStartEventArgs
            {
                Creature = this
            });
            _actionsThisTurn.Clear();
            if (!Conditions.HasCondition(ConditionEnum.Ok)) return;
            if (Conditions.HasCondition(ConditionEnum.Paralyzed)) return;
            _moves = _speed;
            _actionsThisTurn.Add(ActionCategory.Action);
            _actionsThisTurn.Add(ActionCategory.Bonus);
            _actionsThisTurn.Add(ActionCategory.Reaction);
            _actionsThisTurn.Add(ActionCategory.Supplemental);
        }

        public void AddEffect(Effect effect)
        {
            Effects.Add(effect);
            effect.Start(this);
        }

        public void RemoveEffect(Effect effect)
        {
            Effects.Remove(effect);
            effect.End(this);
        }

        public class OnTurnEndEventArgs : EventArgs
        {
            public Creature Creature;
        }

        public class OnTurnStartEventArgs : EventArgs
        {
            public Creature Creature;
        }
    }
}