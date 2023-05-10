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

        public new bool DoAction(Creature actor)
        {
            var enemy = actor.game.PickClosestEnemy(actor);
            while (actor.game.DistanceTo(actor, enemy) > _reach)
                if (!actor.MoveTowards(enemy))
                    break;
            Console.WriteLine($"// {actor.Name} Now at {actor.game.GetLocation(actor)}");

            if (actor.game.DistanceTo(actor, enemy) <= _reach)
            {
                DoAttack(actor, enemy, attackBonus: actor.ProficiencyBonus + actor.Stats[StatEnum.Strength].Bonus(),
                    damageBonus: actor.Stats[StatEnum.Strength].Bonus());
                return true;
            }

            return false;
        }
    }
}