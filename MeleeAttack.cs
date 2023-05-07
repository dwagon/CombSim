using System;

namespace CombSim
{
    public class MeleeAttack : Attack, IAction
    {
        private readonly int _reach;
        
        public MeleeAttack(string name, DamageRoll damageRoll, int reach) : base(name, damageRoll)
        {
            Console.WriteLine("Added MeleeAttack " + name);
            _reach = reach;
        }

        public new bool DoAction(Creature actor)
        {
            Console.WriteLine("MeleeAttack.DoAction(" + actor.Name + ") " + this.Name());
            var enemy = actor.game.PickClosestEnemy(actor);
            var distance = actor.game.DistanceTo(actor, enemy);
            while (distance > _reach)
            {
                if (!actor.MoveTowards(enemy))
                {
                    break;
                }
                distance = actor.game.DistanceTo(actor, enemy);
            }
            
            distance = actor.game.DistanceTo(actor, enemy);
            if (distance <= _reach)
            {
                // Do melee attack here
                Console.WriteLine("Attack!");
                return true;
            }
            return false;
        }
    }
}