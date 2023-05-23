namespace CombSim
{
    public class MeleeAttack : Attack
    {
        private readonly int _reach;
        private readonly MeleeWeapon _weapon;

        public MeleeAttack(string name, DamageRoll damageRoll, int reach = 1, MeleeWeapon weapon = null) : base(name,
            damageRoll)
        {
            _reach = reach;
            _weapon = weapon;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var enemy = actor.PickClosestEnemy();
            var distance = actor.DistanceTo(enemy);
            if (distance <= 2)
            {
                reason = $"Enemy {enemy.Name} adjacent";
                return 4;
            }

            if (distance <= actor.Speed)
            {
                reason = $"Enemy {enemy.Name} within reach if we move";
                return 2;
            }

            reason = $"Enemy {enemy.Name} not within walking distance ({distance} > {actor.Speed})";
            return 0;
        }

        private StatEnum UseStatForAttack(Creature actor)
        {
            var bonusStat = StatEnum.Strength;
            var weapVersatile = false;

            if (_weapon != null) weapVersatile = _weapon.Versatile;

            if ((Versatile || weapVersatile) && actor.Stats[StatEnum.Dexterity] > actor.Stats[StatEnum.Strength])
            {
                bonusStat = StatEnum.Dexterity;
            }

            return bonusStat;
        }

        public override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            actor.MoveWithinReachOfEnemy(_reach, enemy);

            if (actor.Game.DistanceTo(actor, enemy) <= _reach)
            {
                _weapon?.UseWeapon();

                var bonusStat = UseStatForAttack(actor);

                DoAttack(actor, enemy, attackBonus: actor.ProficiencyBonus + actor.Stats[bonusStat].Bonus(),
                    damageBonus: actor.Stats[bonusStat].Bonus());
            }
        }
    }
}