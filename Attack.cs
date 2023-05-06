using System;

namespace CombSim
{
    public class Attack : Action, IAction
    {
        private DamageRoll _dmgRoll;
        protected Attack(string name, DamageRoll damageroll) : base(name, ActionCategory.Action)
        {
            _dmgRoll = damageroll;
            Console.WriteLine("Added Attack action " + name);
        }
        
        public new bool DoAction(Creature actor)
        {
            Console.WriteLine("Attack.DoAction(" + actor.Name + ") " + this.Name());
            return false;
        }
    }
}