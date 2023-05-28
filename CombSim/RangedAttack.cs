namespace CombSim
{
    public class RangedAttack : Attack
    {
        public readonly int LongRange;
        public readonly int ShortRange;

        public RangedAttack(string name, DamageRoll damageRoll, int shortRange, int longRange,
            Weapon weapon = null) :
            base(name, damageRoll, weapon)
        {
            ShortRange = shortRange;
            LongRange = longRange;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            RangedWeapon rangedWeapon = (RangedWeapon)Weapon;
            if (!rangedWeapon.HasAmmunition())
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
            actor.MoveWithinReachOfCreature(ShortRange, enemy);

            var distance = actor.DistanceTo(enemy);

            if (distance <= 1) hasDisadvantage = true;
            else if (distance <= LongRange) hasDisadvantage = true;
            else if (distance > LongRange) return;

            HasAdvantageDisadvantage(actor, enemy, ref hasAdvantage, ref hasDisadvantage);

            Weapon.UseWeapon();
            DoAttack(actor, enemy,
                attackBonus: actor.ProficiencyBonus + actor.Stats[StatEnum.Dexterity].Bonus(),
                damageBonus: actor.Stats[StatEnum.Dexterity].Bonus());
        }
    }
}