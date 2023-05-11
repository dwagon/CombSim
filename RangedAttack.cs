using System;

namespace CombSim
{
    public class RangedAttack : Attack, IAction
    {
        private readonly int _lRange;
        private readonly int _sRange;

        public RangedAttack(string name, DamageRoll damageRoll, int sRange, int lRange) : base(name, damageRoll)
        {
            _sRange = sRange;
            _lRange = lRange;
        }

        public new int GetHeuristic(Creature actor)
        {
            var result = 0;
            var enemy = actor.Game.PickClosestEnemy(actor);
            var distance = actor.Game.DistanceTo(actor, enemy);
            if (distance <= 2) result = 1;
            else if (distance < _sRange) result = 4;
            else if (distance < _lRange) result = 3;
            else result = 2;
            return result;
        }

        public new bool DoAction(Creature actor)
        {
            var enemy = actor.Game.PickClosestEnemy(actor);
            var hasDisadvantage = false;

            while (actor.Game.DistanceTo(actor, enemy) > _sRange)
                if (!actor.MoveTowards(enemy))
                    break;

            var distance = actor.Game.DistanceTo(actor, enemy);
            if (distance <= 1) hasDisadvantage = true;
            else if (distance <= _lRange) hasDisadvantage = true;
            else if (distance > _lRange) return false;

            DoAttack(actor, enemy, hasDisadvantage: hasDisadvantage,
                attackBonus: actor.ProficiencyBonus + actor.Stats[StatEnum.Dexterity].Bonus());
            return true;
        }
    }
}