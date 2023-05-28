using System.Collections.Generic;

namespace CombSim
{
    public class Equipment
    {
        private readonly List<Action> _actions = new List<Action>();
        protected readonly string Name;

        protected Equipment(string name)
        {
            Name = name;
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
        protected Weapon(string name, DamageRoll dmgroll) : base(name)
        {
            DamageRoll = dmgroll;
            Versatile = false;
            Finesse = false;
        }

        public DamageRoll DamageRoll { get; }

        public bool Versatile { get; protected set; }
        public bool Finesse { get; protected set; }

        public virtual void UseWeapon()
        {
        }
    }

    public class MeleeWeapon : Weapon
    {
        private int _reach;

        public MeleeWeapon(string name, DamageRoll dmgroll, int reach) : base(name, dmgroll)
        {
            _reach = reach;
            var action = new MeleeAttack(name, dmgroll, reach, this);
            AddAction(action);
        }
    }

    public class RangedWeapon : Weapon
    {
        private int _ammunition;
        private int _longRange;
        private int _shortRange;

        public RangedWeapon(string name, DamageRoll dmgroll, int shortRange, int longRange, int ammunition = -1) : base(
            name, dmgroll)
        {
            _shortRange = shortRange;
            _longRange = longRange;
            _ammunition = ammunition;
            AddAction(new RangedAttack(name, dmgroll, shortRange, longRange, this));
        }

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
        public readonly int MaxDexBonus;

        public Armour(string name, int armourClass = 0, int armourClassBonus = 0, bool dexBonus = false,
            int maxDexBonus = 2) : base(name)
        {
            ArmourClass = armourClass;
            ArmourClassBonus = armourClassBonus;
            DexBonus = dexBonus;
            MaxDexBonus = maxDexBonus;
        }
    }
}