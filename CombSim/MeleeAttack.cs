using System;

namespace CombSim
{
    public class MeleeAttack : Attack
    {
        private readonly int _reach;

        public MeleeAttack(string name, DamageRoll damageRoll, int reach = 1, Weapon weapon = null) : base(name,
            damageRoll, weapon)
        {
            _reach = reach;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            return heuristic.GetValue(out reason);
        }

        private StatEnum UseStatForAttack(Creature actor)
        {
            var bonusStat = StatEnum.Strength;
            var finesseWeapon = false;

            if (Weapon != null) finesseWeapon = Weapon.Finesse;

            if ((Finesse || finesseWeapon) && actor.Stats[StatEnum.Dexterity] > actor.Stats[StatEnum.Strength])
            {
                bonusStat = StatEnum.Dexterity;
            }

            return bonusStat;
        }

        public override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null)
            {
                Console.WriteLine("No enemies left");
                return;
            }

            actor.MoveWithinReachOfCreature(_reach, enemy);

            if (actor.Game.DistanceTo(actor, enemy) > _reach)
            {
                Console.WriteLine($"// {enemy.Name} still out of reach {actor.DistanceTo(enemy)} > {_reach}");
                return;
            }

            Weapon?.UseWeapon();

            var bonusStat = UseStatForAttack(actor);

            DoAttack(actor, enemy, attackBonus: actor.ProficiencyBonus + actor.Stats[bonusStat].Bonus(),
                damageBonus: actor.Stats[bonusStat].Bonus());
        }
    }
}