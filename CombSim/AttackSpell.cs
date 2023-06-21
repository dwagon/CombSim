using System;

namespace CombSim
{
    public class AttackSpell : Spell
    {
        public AttackSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }

        protected override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            actor.MoveWithinReachOfCreature(Reach, enemy);
            if (actor.Game.DistanceTo(actor, enemy) <= Reach)
            {
                DoAttack(actor, enemy);
            }
        }

        // Simply override this is spell is move towards enemy hurling magic death
        protected virtual void DoAttack(Creature actor, Creature target)
        {
            throw new NotImplementedException();
        }
    }
}