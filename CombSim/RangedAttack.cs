namespace CombSim
{
    public class RangedAttack : Attack
    {
        public readonly int LongRange;
        public readonly int ShortRange;
        private readonly RangedWeapon weapon;

        public RangedAttack(string name, DamageRoll damageRoll, int shortRange, int longRange,
            RangedWeapon weapon = null) :
            base(name, damageRoll)
        {
            ShortRange = shortRange;
            LongRange = longRange;
            this.weapon = weapon;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            int result = 0;
            var heuristic = new Heuristic(actor, this);
            if (!weapon.HasAmmunition())
            {
                reason = "No ammunition";
                return 0;
            }

            return heuristic.GetValue(out reason);
        }

        public override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            var hasAdvantage = false;
            var hasDisadvantage = false;
            actor.MoveWithinReachOfEnemy(ShortRange, enemy);

            var distance = actor.DistanceTo(enemy);

            if (distance <= 1) hasDisadvantage = true;
            else if (distance <= LongRange) hasDisadvantage = true;
            else if (distance > LongRange) return;

            HasAdvantageDisadvantage(actor, enemy, ref hasAdvantage, ref hasDisadvantage);

            weapon.UseWeapon();
            DoAttack(actor, enemy,
                attackBonus: actor.ProficiencyBonus + actor.Stats[StatEnum.Dexterity].Bonus(),
                damageBonus: actor.Stats[StatEnum.Dexterity].Bonus());
        }
    }
}