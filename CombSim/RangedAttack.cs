namespace CombSim
{
    public class RangedAttack : Attack, IAction
    {
        private readonly int _lRange;
        private readonly int _sRange;
        private readonly RangedWeapon weapon;

        public RangedAttack(string name, DamageRoll damageRoll, int sRange, int lRange, RangedWeapon weapon = null) :
            base(name, damageRoll)
        {
            _sRange = sRange;
            _lRange = lRange;
            this.weapon = weapon;
        }

        public override int GetHeuristic(Creature actor)
        {
            int result;
            if (!weapon.HasAmmunition()) return 0;
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return 0;
            var distance = actor.DistanceTo(enemy);
            if (distance <= 2) result = 1;
            else if (distance < _sRange) result = 5;
            else if (distance < _lRange) result = 3;
            else result = 2;
            return result;
        }

        public override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            var hasDisadvantage = false;
            actor.MoveWithinReachOfEnemy(_sRange, enemy);

            var distance = actor.DistanceTo(enemy);
            if (distance <= 1) hasDisadvantage = true;
            else if (distance <= _lRange) hasDisadvantage = true;
            else if (distance > _lRange) return;

            weapon.UseWeapon();
            DoAttack(actor, enemy, hasDisadvantage: hasDisadvantage,
                attackBonus: actor.ProficiencyBonus + actor.Stats[StatEnum.Dexterity].Bonus(),
                damageBonus: actor.Stats[StatEnum.Dexterity].Bonus());
        }
    }
}