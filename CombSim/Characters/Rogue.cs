using System;
using System.Collections.Generic;
using CombSim.Gear;
using Pastel;

namespace CombSim.Characters
{
    public class Rogue : Character
    {
        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>
        {
            { 1, 10 }, { 2, 17 }, { 3, 24 }, { 4, 31 }
        };

        public Rogue(string name, int level = 1, string team = "Rogues") : base(name, team)
        {
            Repr = "R".Pastel(ConsoleColor.Red).PastelBg(ConsoleColor.DarkBlue);

            Level = level;
            MaxHitPoints = _hitPointsAtLevel[level];

            Stats.Add(StatEnum.Strength, new Stat(13));
            Stats.Add(StatEnum.Dexterity, new Stat(16));
            Stats.Add(StatEnum.Constitution, new Stat(14));
            Stats.Add(StatEnum.Intelligence, new Stat(9));
            Stats.Add(StatEnum.Wisdom, new Stat(15));
            Stats.Add(StatEnum.Charisma, new Stat(11));
            AddEquipment(ArmourGear.Leather);
            AddEquipment(RangedWeaponGear.Longbow);
            AddEquipment(MeleeWeaponGear.Shortsword);
            AddEquipment(PotionsGear.HealingPotion);

            // Sneak Attack (+1d6)
            var sneakDamage = new DamageRoll("1d6");
            if (level >= 3)
            {
                // Sneak Attack +2d6
                // Insightful Fighting (Inquisitive archetype)
                sneakDamage = new DamageRoll("2d6");
            }

            if (level >= 4)
            {
                Stats[StatEnum.Dexterity] = new Stat(18);
            }

            AddEffect(new SneakAttack(sneakDamage));
        }

        /* Once per turn, you can deal an extra Xd6 damage to one creature you hit with an attack with
           a finesse or ranged weapon if you have advantage on the attack roll. You don’t need advantage 
           on the attack roll if another enemy of the target is within 5 ft. of it, that enemy isn’t incapacitated,
            and you don’t have disadvantage on the attack roll. */
        private class SneakAttack : Effect
        {
            private readonly DamageRoll _sneakDamage;

            public SneakAttack(DamageRoll sneakDamage) : base("Sneak Attack", hidden: true)
            {
                _sneakDamage = sneakDamage;
            }

            private bool IsStealthAttack(Attack attackAction, Creature actor, Creature target)
            {
                if (attackAction.Weapon == null) return false;

                if (attackAction.HasAdvantage(actor, target))
                {
                    if (attackAction.Weapon.Finesse) return true;
                    if (attackAction is RangedAttack) return true;
                }

                if (attackAction.HasAdvantage(actor, target)) return false;
                foreach (var critter in target.GetNeighbourCreatures())
                {
                    if (critter.Team == actor.Team && critter.IsOk())
                        return true;
                }

                return false;
            }

            // Make the Sneak Attack damage the same type as the weapon used
            private void SetDamageType(Weapon weapon)
            {
                var weaponDamage = weapon.DamageRoll;
                var newDamage = _sneakDamage;
                newDamage.Type = weaponDamage.Type;
            }

            public override void DoAttack(Attack attackAction, Creature actor, Creature target)
            {
                if (!IsStealthAttack(attackAction, actor, target))
                {
                    return;
                }

                SetDamageType(attackAction.Weapon);

                var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name,
                    attackName: "Sneak Attack");
                target.OnHitAttacked?.Invoke(this, new OnHitEventArgs
                {
                    Source = actor,
                    DmgRoll = _sneakDamage,
                    AttackMessage = attackMessage,
                    OnHitSideEffect = null
                });
            }
        }
    }
}