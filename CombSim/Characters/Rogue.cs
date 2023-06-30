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
            { 1, 10 }, { 2, 17 }, { 3, 24 }, { 4, 31 }, { 5, 38 }
        };

        public Rogue(string name, int level = 1, string team = "Rogues") : base(name, level, team)
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
            AddEquipment(new HealingPotion());

            // Sneak Attack (+1d6)
            var sneakDamage = new DamageRoll("1d6");
            if (level >= 3)
            {
                // Sneak Attack +2d6
                // Insightful Fighting (Inquisitive archetype)
                sneakDamage = new DamageRoll("2d6");
                AddEquipment(ArmourGear.LeatherPlusOne);
                AddEquipment(RangedWeaponGear.ShortbowPlusOne);
            }
            else
            {
                AddEquipment(ArmourGear.Leather);
                AddEquipment(RangedWeaponGear.Longbow);
            }

            if (level >= 4)
            {
                Stats[StatEnum.Dexterity] = new Stat(18);
                if (level >= 5)
                {
                    AddEquipment(new GreaterHealingPotion());
                    AddEquipment(new FrostBrandShortsword());
                }
                else
                {
                    AddEquipment(MeleeWeaponGear.ShortswordPlusOne);
                }
            }
            else
            {
                AddEquipment(MeleeWeaponGear.Shortsword);
            }

            if (level >= 5)
            {
                sneakDamage = new DamageRoll("3d6");
            }

            AddEffect(new SneakAttack(sneakDamage));
        }

        /* Once per turn, you can deal an extra Xd6 damage to one creature you hit with an attack with
           a finesse or ranged weapon if you have advantage on the attack roll. You don’t need advantage 
           on the attack roll if another enemy of the target is within 5 ft. of it, that enemy isn’t incapacitated,
            and you don’t have disadvantage on the attack roll. */
        internal class SneakAttack : Effect
        {
            private readonly DamageRoll _sneakDamage;

            public SneakAttack(DamageRoll sneakDamage) : base("Sneak Attack", hidden: true)
            {
                _sneakDamage = sneakDamage;
            }

            internal bool IsSneakAttack(Attack attackAction, Creature actor, Creature target)
            {
                if (attackAction.Weapon == null) return false;

                if (attackAction.HasAdvantage(actor, target))
                {
                    if (attackAction.Weapon.Finesse) return true;
                    if (attackAction is RangedAttack) return true;
                }

                if (attackAction.HasDisadvantage(actor, target)) return false;
                if (HasAdjacentAlly(actor, target)) return true;

                return false;
            }

            internal bool HasAdjacentAlly(Creature actor, Creature target)
            {
                foreach (var critter in target.GetNeighbourCreatures())
                {
                    if (critter != actor && critter.Team == actor.Team && critter.IsOk())
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
                if (!IsSneakAttack(attackAction, actor, target))
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