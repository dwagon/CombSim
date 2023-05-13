using System;
using System.Collections.Generic;
using System.Linq;

namespace CombSim
{
    public class Creature
    {
        private readonly Dictionary<string, Spell> _spells;
        private readonly List<Action> _actions;
        private readonly HashSet<ActionCategory> _actionsThisTurn;
        private readonly List<Equipment> _equipment;
        private readonly int _speed;
        protected readonly Conditions Conditions;
        public readonly Dictionary<StatEnum, Stat> Stats;
        private int _moves;
        protected StatEnum SpellCastingAbility;

        private int _setArmourClass = -1;
        protected int HitPoints;
        protected int MaxHitPoints;
        public EventHandler<OnAttackedEventArgs> OnAttacked;
        public EventHandler<OnTurnEndEventArgs> OnTurnEnd;
        public EventHandler<OnTurnStartEventArgs> OnTurnStart;
        public int ProficiencyBonus = 2;
        protected string Repr;
        protected List<DamageTypeEnums> Vulnerable;
        protected List<DamageTypeEnums> Resistant;
        protected List<DamageTypeEnums> Immune;
        protected Effects Effects;
        protected List<Damage> DamageReceived;

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
        }

        public string Name { get; protected set; }
        public string Team { get; protected set; }

        public int ArmourClass
        {
            get => CalcArmourClass();
            protected set => _setArmourClass = value;
        }

        public int HitPointsDown()
        {
            return MaxHitPoints - HitPoints;
        }

        public int PercentHitPoints()
        {
            return (int)(100 * ((float)HitPoints / MaxHitPoints));
        }

        public Game Game { get; private set; }

        public virtual void Initialise()
        {
            HitPoints = MaxHitPoints;
            Conditions.SetCondition(ConditionEnum.Ok);
            OnAttacked += Attacked;
        }

        public int SpellAttackModifier()
        {
            return ProficiencyBonus + Stats[SpellCastingAbility].Bonus();
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

            Damage dmg;
            if (e.CriticalHit)
            {
                dmg = e.DmgRoll.Roll(max: true) + e.DmgRoll.Roll();
            }
            else
            {
                dmg = e.DmgRoll.Roll();
            }

            dmg = ModifyDamageForVulnerabilityOrResistance(dmg);
            e.AttackMessage.Result = $"Hit for {dmg}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
            TakeDamage(dmg);
            e.OnHitSideEffect(this);
        }

        private Damage ModifyDamageForVulnerabilityOrResistance(Damage dmg)
        {
            if (Vulnerable.Contains(dmg.type))
            {
                dmg *= 2;
            }
            else if (Resistant.Contains(dmg.type))
            {
                dmg /= 2;
            }
            else if (Immune.Contains(dmg.type))
            {
                dmg = new Damage(0, dmg.type);
            }

            return dmg;
        }

        public void AddSpell(Spell spell)
        {
            _spells[spell.Name()] = spell;
            AddAction(spell);
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

        public new string ToString()
        {
            return $"{Name} HP: {HitPoints}/{MaxHitPoints} {Conditions} {Effects}";
        }

        // Return all the possible actions
        private List<IAction> PossibleActions(ActionCategory actionCategory)
        {
            var actions = new List<IAction>();
            foreach (var action in _actions)
                if (action.Category == actionCategory && _actionsThisTurn.Contains(actionCategory))
                    actions.Add(action);

            Console.WriteLine($"// {Name} Possible {actionCategory} Actions: {String.Join(", ", actions)}");
            return actions;
        }

        protected void AddAction(Action action)
        {
            _actions.Add(action);
        }

        public void RemoveAction(Action action)
        {
            _actions.Remove(action);
        }

        public int Heal(int hitpoints)
        {
            hitpoints = Math.Min(hitpoints, HitPointsDown());
            HitPoints += hitpoints;

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

            return hitpoints;
        }

        public Creature PickClosestEnemy()
        {
            return Game.PickClosestEnemy(this);
        }

        public float DistanceTo(Creature enemy)
        {
            return Game.DistanceTo(this, enemy);
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
                DoActionCategory(ActionCategory.Bonus);
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

        private IAction PickActionToDo(ActionCategory actionCategory)
        {
            if (!_actionsThisTurn.Contains(actionCategory)) return null;

            var sortableActions = new List<(int heuristic, IAction action)>();
            var possibleActions = PossibleActions(actionCategory);

            foreach (var action in possibleActions)
            {
                var heuristic = action.GetHeuristic(this);
                if (heuristic > 0) sortableActions.Add((heuristic, action));
            }

            if (!sortableActions.Any()) return null;

            sortableActions.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            return sortableActions.Last().Item2;
        }

        private void TakeDamage(Damage damage)
        {
            HitPoints -= damage.hits;
            DamageReceived.Add(damage);
            if (HitPoints <= 0) FallenUnconscious();
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
            NarrationLog.LogMessage($"{Name} has died");
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

        public class OnAttackedEventArgs : EventArgs
        {
            public IAction Action;
            public bool CriticalHit;
            public bool CriticalMiss;
            public DamageRoll DmgRoll;
            public Creature Source;
            public int ToHit;
            public AttackMessage AttackMessage;
            public Action<Creature> OnHitSideEffect;
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