using System.Collections.Generic;

namespace CombSim
{
    public class Equipment : IModifier
    {
        private readonly List<Action> _actions = new List<Action>();
        public readonly string Name;

        protected Equipment(string name)
        {
            Name = name;
        }

        public virtual int ArmourModification(Attack attack)
        {
            return 0;
        }

        public virtual int SavingThrowModification(StatEnum stat)
        {
            return 0;
        }

        public virtual bool HasAdvantageAgainstMe(Creature actor, Creature victim)
        {
            return false;
        }

        public virtual bool HasDisadvantageAgainstMe(Creature actor, Creature victim)
        {
            return false;
        }

        public virtual void DoAttack(Attack attackAction, Creature actor, Creature victim)
        {
        }

        public virtual void Equip(Creature owner)
        {
        }

        protected void AddAction(Action act)
        {
            _actions.Add(act);
        }

        public List<Action> GetActions()
        {
            return _actions;
        }
    }

    public class Weapon : Equipment
    {
        protected Action action;

        protected Weapon(string name, DamageRoll damageRoll, int magicBonus = 0) : base(name)
        {
            DamageRoll = damageRoll;
            Versatile = false;
            Finesse = false;
            MagicBonus = magicBonus;
        }

        public int MagicBonus { get; }
        public bool Versatile { get; protected set; }
        public bool Finesse { get; protected set; }

        public DamageRoll DamageRoll { get; }

        public virtual void UseWeapon()
        {
        }

        public virtual void SideEffect(Creature target, Creature actor)
        {
        }

        // What modifier to the heuristic does the side effect have
        public virtual int SideEffectHeuristic()
        {
            return 0;
        }
    }

    public class MeleeWeapon : Weapon
    {
        public MeleeWeapon(string name, DamageRoll damageRoll, int reach = 5 / 5, int magicBonus = 0,
            bool finesse = false) :
            base(name, damageRoll, magicBonus)
        {
            Reach = reach;
            Finesse = finesse;
            action = new MeleeAttack(this);
            AddAction(action);
        }

        public int Reach { get; }
    }

    public class RangedWeapon : Weapon
    {
        private int _ammunition;

        public RangedWeapon(string name, DamageRoll dmgroll, int shortRange, int longRange, int magicBonus = 0,
            int ammunition = -1) : base(
            name, dmgroll, magicBonus)
        {
            ShortRange = shortRange;
            LongRange = longRange;
            _ammunition = ammunition;
            action = new RangedAttack(this);
            AddAction(action);
        }

        public int LongRange { get; }
        public int ShortRange { get; }

        public void AddAmmunition(int amount)
        {
            if (_ammunition < 0)
            {
                _ammunition = 0;
            }

            _ammunition += amount;
        }

        public bool HasAmmunition()
        {
            return _ammunition != 0;
        }

        public int GetAmmunition()
        {
            return _ammunition;
        }

        private bool UseAmmunition()
        {
            if (_ammunition <= 0)
            {
                return false;
            }

            _ammunition--;
            return true;
        }

        public override void UseWeapon()
        {
            UseAmmunition();
        }
    }

    public class Armour : Equipment
    {
        public readonly int ArmourClass;
        public readonly int ArmourClassBonus;
        public readonly bool DexBonus;
        public readonly int MagicBonus;
        public readonly int MaxDexBonus;

        public Armour(string name, int armourClass = 0, int armourClassBonus = 0, bool dexBonus = false,
            int maxDexBonus = 99, int magicBonus = 0) : base(name)
        {
            ArmourClass = armourClass;
            ArmourClassBonus = armourClassBonus;
            DexBonus = dexBonus;
            MaxDexBonus = maxDexBonus;
            MagicBonus = magicBonus;
        }
    }
}