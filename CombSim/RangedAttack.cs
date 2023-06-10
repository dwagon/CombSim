namespace CombSim
{
    public class RangedAttack : Attack
    {
        public readonly int LongRange;
        public readonly int ShortRange;

        protected RangedAttack(string name, DamageRoll damageRoll, int shortRange, int longRange) :
            base(name, damageRoll)
        {
            ShortRange = shortRange;
            LongRange = longRange;
        }

        public RangedAttack(RangedWeapon weapon) : base(weapon)
        {
            ShortRange = weapon.ShortRange;
            LongRange = weapon.LongRange;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            heuristic.AddRepeat(actor.AttacksPerAction);
            RangedWeapon rangedWeapon = (RangedWeapon)Weapon;
            if (rangedWeapon != null && !rangedWeapon.HasAmmunition())
            {
                reason = "No ammunition";
                return 0;
            }

            var damageBonus = actor.Stats[StatEnum.Dexterity].Bonus();
            if (Weapon != null)
            {
                damageBonus += Weapon.MagicBonus;
            }

            heuristic.AddDamage(damageBonus);

            return heuristic.GetValue(out reason);
        }

        public override void DoAction(Creature actor)
        {
            for (var attack = 0; attack < actor.AttacksPerAction; attack++)
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

                Weapon?.UseWeapon();
                DoAttack(actor, enemy,
                    attackBonus: actor.ProficiencyBonus + actor.Stats[StatEnum.Dexterity].Bonus(),
                    damageBonus: actor.Stats[StatEnum.Dexterity].Bonus());
            }
        }
    }
}