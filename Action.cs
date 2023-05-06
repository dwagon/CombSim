using System;

namespace CombSim
{
    public enum ActionCategory
    {
        Action,
        Bonus,
        Move,
        Reaction
    }
    
    public class Action
    {
        public string Name { get; private set; }
        public ActionCategory Category { get; private set; }

        protected Action(string name, ActionCategory category)
        {
            Name = name;
            Category = category;
        }

        public bool DoAction(Creature actor)
        {
            Console.WriteLine("Action.DoAction(" + actor.Name + ") " + this.Name);
            return false;
        }
    }

    public class Attack : Action
    {
        private DamageRoll _dmgRoll;
        protected Attack(string name, DamageRoll damageroll) : base(name, ActionCategory.Action)
        {
            _dmgRoll = damageroll;
        }
        
        public new bool DoAction(Creature actor)
        {
            Console.WriteLine("Attack.DoAction(" + actor.Name + ") " + this.Name);
            return false;
        }
    }

    public class MeleeAttack : Attack
    {
        private int _reach;
        
        public MeleeAttack(string name, DamageRoll damageroll, int reach) : base(name, damageroll)
        {
            _reach = reach;
        }

        public new bool DoAction(Creature actor)
        {
            Console.WriteLine("MeleeAttack.DoAction(" + actor.Name + ") " + this.Name);
            Creature enemy = actor.game.PickClosestEnemy(actor);
            float distance = actor.game.DistanceTo(actor, enemy);
            while (distance > _reach)
            {
                if (!actor.MoveTowards(enemy.Location))
                {
                    break;
                }
                distance = actor.game.DistanceTo(actor, enemy);
            }
            
            distance = actor.game.DistanceTo(actor, enemy);
            if (distance <= _reach)
            {
                // Do melee attack here
                return true;
            }
            return false;
        }
    }

    public class RangedAttack : Attack
    {
        private int _srange;
        private int _lrange;
        
        public RangedAttack(string name, DamageRoll damageroll, int srange, int lrange) : base(name, damageroll)
        {
            _srange = srange;
            _lrange = lrange;
        }
    }
}