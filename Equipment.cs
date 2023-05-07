using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CombSim
{
    public class Equipment
    {
        private string _name;
        private List<Action> _actions = new List<Action>();

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
        private int short_range;
        private int long_range;

        public RangedWeapon(string name, DamageRoll dmgroll, int short_range, int long_range) : base(name, dmgroll)
        {
            this.short_range = short_range;
            this.long_range = long_range;
            AddAction(new RangedAttack(name, dmgroll, short_range, long_range));
        }
    }
}