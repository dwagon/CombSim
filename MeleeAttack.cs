using System;

namespace CombSim
{
    public class MeleeAttack : Attack, IAction
    {
        private int _reach;
        
        public MeleeAttack(string name, DamageRoll damageroll, int reach) : base(name, damageroll)
        {
            Console.WriteLine("Added MeleeAttack " + name);
            _reach = reach;
        }

        public new bool DoAction(Creature actor)
        {
            Console.WriteLine("MeleeAttack.DoAction(" + actor.Name + ") " + this.Name());
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
}