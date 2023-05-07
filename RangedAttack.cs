namespace CombSim
{

    public class RangedAttack : Attack, IAction
    {
        private int _sRange;
        private int _lRange;
        
        public RangedAttack(string name, DamageRoll damageRoll, int sRange, int lRange) : base(name, damageRoll)
        {
            _sRange = sRange;
            _lRange = lRange;
        }
        
        public new bool DoAction(Creature actor)
        {
            var enemy = actor.game.PickClosestEnemy(actor);
            while (actor.game.DistanceTo(actor, enemy) > _sRange)
            {
                if (!actor.MoveTowards(enemy)) break;
            }
            
            if (actor.game.DistanceTo(actor, enemy) <= _sRange)
            {
                DoAttack(enemy);
                return true;
            }
            return false;
        }
    }
}