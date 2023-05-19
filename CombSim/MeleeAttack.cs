using System;

namespace CombSim
{
    public class MeleeAttack : Attack, IAction
    {
        private readonly int _reach;

        public MeleeAttack(string name, DamageRoll damageRoll, int reach=1) : base(name, damageRoll)
        {
            _reach = reach;
        }

        public override int GetHeuristic(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return 0;
            if (actor.DistanceTo(enemy) <= 2)
            {
                return 4;
            }

            return 1;
        }

        public new bool DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            var oldLocation = actor.GetLocation();
            if (enemy == null) return false;
            actor.MoveWithinReachOfEnemy(_reach, enemy);

            if (actor.Game.DistanceTo(actor, enemy) <= _reach)
            {
                DoAttack(actor, enemy, attackBonus: actor.ProficiencyBonus + actor.Stats[StatEnum.Strength].Bonus(),
                    damageBonus: actor.Stats[StatEnum.Strength].Bonus());
                return true;
            }

            return false;
        }
    }
}