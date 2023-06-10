using System;

namespace CombSim
{
    public class MeleeAttack : Attack
    {
        protected MeleeAttack(string name, DamageRoll damageRoll, int reach = 1) : base(name, damageRoll)
        {
            Reach = reach;
        }

        public MeleeAttack(MeleeWeapon weapon) : base(weapon)
        {
            Reach = weapon.Reach;
        }

        public int Reach { get; }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            heuristic.AddRepeat(actor.AttacksPerAction);
            var bonusStat = UseStatForAttack(actor);
            var damageBonus = actor.Stats[bonusStat].Bonus();
            if (Weapon != null)
            {
                damageBonus += Weapon.MagicBonus;
            }

            heuristic.AddDamage(damageBonus);
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
            for (var attack = 0; attack < actor.AttacksPerAction; attack++)
            {
                var enemy = actor.PickClosestEnemy();
                if (enemy == null)
                {
                    Console.WriteLine("No enemies left");
                    return;
                }

                actor.MoveWithinReachOfCreature(Reach, enemy);

                if (actor.Game.DistanceTo(actor, enemy) > Reach)
                {
                    Console.WriteLine($"// {enemy.Name} still out of reach {actor.DistanceTo(enemy)} > {Reach}");
                    return;
                }

                Weapon?.UseWeapon();

                var bonusStat = UseStatForAttack(actor);

                DoAttack(actor, enemy, attackBonus: actor.ProficiencyBonus + actor.Stats[bonusStat].Bonus(),
                    damageBonus: actor.Stats[bonusStat].Bonus());
            }
        }
    }
}