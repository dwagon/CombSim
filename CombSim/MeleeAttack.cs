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
            var heuristic = new Heuristic(actor, this);
            return heuristic.GetValue(out reason);
        }

        private StatEnum UseStatForAttack(Creature actor)
        {
            var bonusStat = StatEnum.Strength;
            var versatileWeapon = false;

            if (_weapon != null) versatileWeapon = _weapon.Versatile;

            if ((Versatile || versatileWeapon) && actor.Stats[StatEnum.Dexterity] > actor.Stats[StatEnum.Strength])
            {
                bonusStat = StatEnum.Dexterity;
            }

            return bonusStat;
        }

        public override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            actor.MoveWithinReachOfCreature(_reach, enemy);

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