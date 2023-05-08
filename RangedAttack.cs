namespace CombSim
{

    public class RangedAttack : Attack, IAction
    {
        private readonly int _sRange;
        private readonly int _lRange;
        
        public RangedAttack(string name, DamageRoll damageRoll, int sRange, int lRange) : base(name, damageRoll)
        {
            _sRange = sRange;
            _lRange = lRange;
        }
        
        public new bool DoAction(Creature actor)
        {
            var enemy = actor.game.PickClosestEnemy(actor);
            bool hasDisadvantage = false;
            
            while (actor.game.DistanceTo(actor, enemy) > _sRange)
            {
                if (!actor.MoveTowards(enemy)) break;
            }

            var distance = actor.game.DistanceTo(actor, enemy);
            if (distance <= 1) hasDisadvantage = true;
            else if (distance <= _lRange) hasDisadvantage = true;
            else if (distance > _lRange) return false;
            
            DoAttack(actor, enemy, hasDisadvantage: hasDisadvantage);
            return true;
        }
    }
}