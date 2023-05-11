using System;

namespace CombSim
{
    public class MeleeAttack : Attack, IAction
    {
        private readonly int _reach;

        public MeleeAttack(string name, DamageRoll damageRoll, int reach) : base(name, damageRoll)
        {
            _reach = reach;
        }

        public new int GetHeuristic(Creature actor)
        {
            var enemy = actor.Game.PickClosestEnemy(actor);
            if (actor.Game.DistanceTo(actor, enemy) <= 2)
            {
                return 4;
            }

            return 1;
        }

        public new bool DoAction(Creature actor)
        {
            var enemy = actor.Game.PickClosestEnemy(actor);
            while (actor.Game.DistanceTo(actor, enemy) > _reach)
                if (!actor.MoveTowards(enemy))
                    break;
            Console.WriteLine($"// {actor.Name} Now at {actor.Game.GetLocation(actor)}");

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