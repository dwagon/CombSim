using System;
using System.Collections.Generic;

namespace CombSim
{
    public class Equipment
    {
        private readonly List<Action> _actions = new List<Action>();
        protected string _name;

        public Equipment(string name)
        {
            _name = name;
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
        private DamageRoll dmgRoll;

        protected Weapon(string name, DamageRoll dmgroll) : base(name)
        {
            dmgRoll = dmgroll;
        }
    }
    
    public class MeleeWeapon : Weapon
    {
        private int reach;

        public MeleeWeapon(string name, DamageRoll dmgroll, int reach) : base(name, dmgroll)
        {
            this.reach = reach;
            AddAction(new MeleeAttack(name, dmgroll, reach));
        }
    }

    public class RangedWeapon : Weapon
    {
        private int long_range;
        private int short_range;

        public RangedWeapon(string name, DamageRoll dmgroll, int short_range, int long_range) : base(name, dmgroll)
        {
            this.short_range = short_range;
            this.long_range = long_range;
            AddAction(new RangedAttack(name, dmgroll, short_range, long_range));
        }
    }

    public class Armour : Equipment
    {
        public int ArmourClass;
        public int ArmourClassBonus;
        public bool DexBonus;
        public int MaxDexBonus;

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