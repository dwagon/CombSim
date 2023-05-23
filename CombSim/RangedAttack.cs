namespace CombSim
{
    public class RangedAttack : Attack
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

        public override int GetHeuristic(Creature actor, out string reason)
        {
            int result = 0;
            if (!weapon.HasAmmunition())
            {
                reason = "No ammunition";
                return 0;
            }

            var enemy = actor.PickClosestEnemy();
            var distance = actor.DistanceTo(enemy);
            if (distance <= 2)
            {
                reason = "Adjacent";
                result = 1;
            }
            else if (distance < _sRange)
            {
                reason = $"Within Short range {_sRange}";
                result = 5;
            }
            else if (distance < _lRange)
            {
                reason = $"Within Long range {_lRange}";
                result = 3;
            }
            else if (distance < _lRange + actor.Speed)
            {
                reason = $"Within Long range if we move";
                result = 1;
            }
            else
            {
                reason = "Nothing in range";
            }

            return result;
        }

        public override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            var hasAdvantage = false;
            var hasDisadvantage = false;
            actor.MoveWithinReachOfEnemy(_sRange, enemy);

            var distance = actor.DistanceTo(enemy);

            if (distance <= 1) hasDisadvantage = true;
            else if (distance <= _lRange) hasDisadvantage = true;
            else if (distance > _lRange) return;

            HasAdvantageDisadvantage(actor, enemy, ref hasAdvantage, ref hasDisadvantage);

            weapon.UseWeapon();
            DoAttack(actor, enemy,
                attackBonus: actor.ProficiencyBonus + actor.Stats[StatEnum.Dexterity].Bonus(),
                damageBonus: actor.Stats[StatEnum.Dexterity].Bonus());
        }
    }
}