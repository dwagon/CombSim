using System;
using System.Collections.Generic;

namespace CombSim
{
    public partial class Creature
    {
        public static EventHandler<OnAnyBeingKilledEventArgs> OnAnyBeingKilled;
        private readonly List<Action> _actions;
        private readonly HashSet<ActionCategory> _actionsThisTurn;
        private readonly List<Damage> _damageInflicted;
        private readonly List<Equipment> _equipment;
        private readonly Dictionary<string, Spell> _spells;
        protected readonly Conditions Conditions;
        protected readonly List<Damage> DamageReceived;
        protected readonly List<DamageTypeEnums> Immune;
        public readonly Modifiers Modifiers;
        protected readonly List<DamageTypeEnums> Resistant;
        public readonly Dictionary<StatEnum, Stat> Stats;
        protected readonly List<DamageTypeEnums> Vulnerable;
        private int _setArmourClass = -1;
        protected int HitPoints;
        protected int MaxHitPoints;
        public int Moves;
        public EventHandler<OnMovingEventArgs> OnMoving;
        public EventHandler<OnTurnEndEventArgs> OnTurnEnd;
        public EventHandler<OnTurnStartEventArgs> OnTurnStart;
        public int ProficiencyBonus = 2;
        protected string Repr;
        protected StatEnum SpellCastingAbility;

        public Creature(string name, string team = "")
        {
            Name = name;
            Team = team;
            Speed = 6;
            Stats = new Dictionary<StatEnum, Stat>();
            Conditions = new Conditions();
            Effects = new Effects();
            _equipment = new List<Equipment>();
            _actions = new List<Action>();
            _actionsThisTurn = new HashSet<ActionCategory>();
            _spells = new Dictionary<string, Spell>();
            Vulnerable = new List<DamageTypeEnums>();
            Modifiers = new Modifiers();
            Resistant = new List<DamageTypeEnums>();
            Immune = new List<DamageTypeEnums>();
            DamageReceived = new List<Damage>();
            _damageInflicted = new List<Damage>();
            CriticalHitRoll = 20;
            Attributes = new HashSet<Attribute>();
            AttacksPerAction = 1;
            AddAction(new DashAction());
        }

        public int AttacksPerAction { get; protected set; }

        public Effects Effects { get; }

        public int Speed { get; protected set; }

        protected HashSet<Attribute> Attributes { get; private set; }
        public int CriticalHitRoll { get; protected set; }

        public string Name { get; }
        public string Team { get; }

        public Game Game { get; private set; }

        public void DamageMaxHealth(int damage)
        {
            MaxHitPoints -= damage;
        }

        protected bool HasAttribute(Attribute attribute)
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

        public bool HasAdvantageAgainstMe(Creature target)
        {
            if (HasCondition(ConditionEnum.Restrained))
            {
                Console.WriteLine($"// Advantage due to being restrained");
                return true;
            }

            return Modifiers.HasAdvantageAgainstMe(this, target);
        }

        public bool HasDisadvantageAgainstMe(Creature target)
        {
            return Modifiers.HasDisadvantageAgainstMe(this, target);
        }

        private List<Location> GetNeighbourLocations()
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

        protected void ArmourClass(int ac)
        {
            _setArmourClass = ac;
        }

        public int ArmourClass(Attack attack = null)
        {
            if (_setArmourClass >= 0) return _setArmourClass;

            var baseAc = 10; // AC you have just by default
            var ac = baseAc;
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

                    acBonus += armour.ArmourClassBonus;
                    acBonus += armour.MagicBonus;
                }
            }

            acBonus += Modifiers.ArmourModification(attack);
            var result = ac + acBonus + GetDexAcBonus(dexBonus, maxDexBonus);
            if (result == 0) result = baseAc + Stats[StatEnum.Dexterity].Bonus(); // Not wearing any armour
            return result;
        }

        public List<string> GetGearList()
        {
            var result = new List<string>();
            foreach (var gear in _equipment)
            {
                result.Add(gear.Name);
            }

            return result;
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

        public void AddEquipment(Equipment gear)
        {
            _equipment.Add(gear);
            Modifiers.Add(gear);
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
        private bool MoveTo(Location location)
        {
            if (Moves <= 0)
                return false;
            OnMoving?.Invoke(this, new OnMovingEventArgs
            {
                mover = this,
                location = location
            });
            if (IsAlive()) // Can die due to area effects, etc.
            {
                Game.Move(this, location);
                Moves--;
            }

            return true;
        }

        // Is this creature able to continue participating in combat
        public bool IsOk()
        {
            if (Conditions.HasCondition(ConditionEnum.Dead)) return false;
            if (Conditions.HasCondition(ConditionEnum.Unconscious)) return false;
            if (Conditions.HasCondition(ConditionEnum.Stable)) return false;
            return true;
        }

        // Is this creature alive?
        public bool IsAlive()
        {
            if (Conditions.HasCondition(ConditionEnum.Dead)) return false;
            return true;
        }

        public Location GetLocation()
        {
            return Game.GetLocation(this);
        }

        public new virtual string ToString()
        {
            var output = $"{Name} AC: {ArmourClass()}; HP: {HitPoints}/{MaxHitPoints};";
            output += $" {Conditions};";
            output += $" {Effects}";
            return output;
        }

        public string DamageReport(out int total)
        {
            var result = new List<string>();
            var damageTypes = new Dictionary<DamageTypeEnums, int>();
            total = 0;
            foreach (var damage in _damageInflicted)
            {
                if (!damageTypes.ContainsKey(damage.type))
                {
                    damageTypes[damage.type] = 0;
                }

                damageTypes[damage.type] += damage.hits;
                total += damage.hits;
            }

            foreach (var kvp in damageTypes)
            {
                result.Add($"{kvp.Key}: {kvp.Value}");
            }

            result.Add($"Total: {total}");
            if (total == 0) return "None";
            return String.Join("; ", result);
        }

        public void Heal(int hitPoints, string reason = "")
        {
            string reasonString = "";
            hitPoints = Math.Min(hitPoints, HitPointsDown());
            HitPoints += hitPoints;
            if (reason != "")
            {
                reasonString = $"by {reason}";
            }

            NarrationLog.LogMessage($"{Name} healed {hitPoints} {reasonString}");

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
        }

        public float DistanceTo(Creature enemy)
        {
            return Game.DistanceTo(this, enemy);
        }

        public IEnumerable<Creature> GetCreaturesInCone(int coneSize, GridDirection direction)
        {
            return Game.GetCreaturesInCone(GetLocation(), coneSize, direction);
        }

        public IEnumerable<Creature> GetCreaturesInCircle(int radius)
        {
            return Game.GetCreaturesInCircle(GetLocation(), radius);
        }

        public void DoActionCategory(ActionCategory actionCategory, bool force = false)
        {
            var enemy = PickClosestEnemy();
            if (enemy == null) return;
            if (force) _actionsThisTurn.Add(actionCategory);
            PerformAction(PickActionToDo(actionCategory));
        }

        public void TakeTurn()
        {
            Console.WriteLine(
                $"\n// {Name} -------------------------------------------------------------------------------------------");
            TurnStart();
            if (IsOk())
            {
                DoActionCategory(ActionCategory.Action);
                DoActionCategory(ActionCategory.Supplemental);
                DoActionCategory(ActionCategory.Bonus);
            }

            TurnEnd();
        }

        private void TurnEnd()
        {
            if (IsOk())
            {
                SpendRestOfMovement();
            }

            OnTurnEnd?.Invoke(this, new OnTurnEndEventArgs
            {
                Creature = this
            });
        }

        // Spend remaining movement moving closer to enemies, but don't get too close
        private void SpendRestOfMovement()
        {
            const int minimumDistance = 5;
            var enemy = PickClosestEnemy();
            MoveWithinReachOfCreature(minimumDistance, enemy);
        }

        private void PerformAction(Action action)
        {
            if (action is null) return;

            Console.WriteLine($"// {Name} doing {action.Name()}");
            action.PerformAction(this);
            _actionsThisTurn.Remove(action.Category);
        }

        protected virtual void FallenUnconscious()
        {
        }

        public bool MakeSavingThrow(StatEnum stat, int dc, out int roll)
        {
            Console.Write($"// {Name} does {stat} saving vs DC {dc} - ");
            roll = Stats[stat].Roll();
            roll += Modifiers.SavingThrowModification(stat);

            if (HasCondition(ConditionEnum.Paralyzed))
            {
                if (stat == StatEnum.Strength || stat == StatEnum.Dexterity) return false;
            }

            if (HasCondition(ConditionEnum.Restrained))
            {
                if (stat == StatEnum.Dexterity)
                {
                    var roll2 = Stats[stat].Roll();
                    Console.WriteLine($"// Dex saved with disadvantage due to restrained");
                    roll = Math.Min(roll2, roll);
                }
            }

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

        // Used only for testing - Reset things that are normally hidden
        public void TestReset()
        {
            Console.WriteLine("// Resetting for testing purposes");
            _actionsThisTurn.Add(ActionCategory.Action);
        }

        // Dynamically make the creature resistant to a damage type
        public void AddResistance(DamageTypeEnums damageType)
        {
            Resistant.Add(damageType);
        }

        protected virtual void TurnStart()
        {
            Moves = Speed;

            OnTurnStart?.Invoke(this, new OnTurnStartEventArgs
            {
                Creature = this
            });
            if (HasCondition(ConditionEnum.Prone))
            {
                Moves /= 2;
                RemoveCondition(ConditionEnum.Prone);
                Console.WriteLine("// Getting up from prone");
            }

            if (HasCondition(ConditionEnum.Restrained))
            {
                Moves = 0;
            }

            _actionsThisTurn.Clear();
            if (!Conditions.HasCondition(ConditionEnum.Ok)) return;
            if (Conditions.HasCondition(ConditionEnum.Paralyzed)) return;
            _actionsThisTurn.Add(ActionCategory.Action);
            _actionsThisTurn.Add(ActionCategory.Bonus);
            _actionsThisTurn.Add(ActionCategory.Reaction);
            _actionsThisTurn.Add(ActionCategory.Supplemental);
        }

        public void AddEffect(Effect effect)
        {
            Effects.Add(effect);
            effect.Start(this);
            Modifiers.Add(effect);
        }

        public void RemoveEffect(Effect effect)
        {
            Effects.Remove(effect);
            Modifiers.Remove(effect);
            Console.WriteLine($"// Removing effect {effect.Name}");
            effect.End(this);
        }

        public virtual int HealingBonus()
        {
            return 0;
        }

        public void MoveWithinReachOfCreature(int reach, Creature enemy)
        {
            var oldLocation = GetLocation();
            if (enemy == null) return;
            var route = Game.GetRouteTowards(this, enemy);
            foreach (var location in route)
            {
                if (DistanceTo(enemy) <= reach)
                    break;
                if (!MoveTo(location))
                    break;
                if (!IsAlive())
                    break;
            }

            if (oldLocation != GetLocation())
            {
                Console.WriteLine($"// {Name} moved from {oldLocation} to {GetLocation()}");
            }
        }

        public Creature PickClosestEnemy()
        {
            return Game.PickClosestEnemy(this);
        }

        public List<Creature> GetAllAllies()
        {
            return Game.GetAllAllies(this);
        }

        public bool HasEffect(string name)
        {
            return Effects.HasEffect(name);
        }

        public Effect GetEffectByName(string name)
        {
            return Effects.GetEffectByName(name);
        }
    }
}