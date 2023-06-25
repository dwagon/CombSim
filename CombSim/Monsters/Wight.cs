using System;
using CombSim.Gear;

namespace CombSim.Monsters
{
    public class Wight : Monster
    {
        public Wight(string name, string team = "Wights") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(15));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(16));
            Stats.Add(StatEnum.Intelligence, new Stat(10));
            Stats.Add(StatEnum.Wisdom, new Stat(13));
            Stats.Add(StatEnum.Charisma, new Stat(15));
            Repr = "W";
            HitDice = "6d8+18";

            AddEquipment(ArmourGear.StuddedLeather);
            Resistant.Add(DamageTypeEnums.Necrotic);
            Resistant.Add(DamageTypeEnums.Bludgeoning);
            Resistant.Add(DamageTypeEnums.Piercing);
            Resistant.Add(DamageTypeEnums.Slashing);
            Immune.Add(DamageTypeEnums.Poison);
            ProficiencyBonus = 2;

            var multiLongswordAttack = new MultiAttack("Multi Longsword Attack", ActionCategory.Action);
            multiLongswordAttack.AddAttack(MeleeWeaponGear.Longsword);
            multiLongswordAttack.AddAttack(MeleeWeaponGear.Longsword);
            AddAction(multiLongswordAttack);

            var multiLongbowAttack = new MultiAttack("Multi Longbow Attack", ActionCategory.Action);
            multiLongbowAttack.AddAttack(RangedWeaponGear.Longbow);
            multiLongbowAttack.AddAttack(RangedWeaponGear.Longbow);
            AddAction(multiLongbowAttack);

            var multiLifeAttack = new MultiAttack("Multi Life Drain Attack", ActionCategory.Action);
            multiLifeAttack.AddAttack(MeleeWeaponGear.Longsword);
            multiLifeAttack.AddAttack(new LifeDrain());
            AddAction(multiLifeAttack);
        }

        private class LifeDrain : MeleeAttack
        {
            public LifeDrain() : base("Life Drain", new DamageRoll("1d6", DamageTypeEnums.Necrotic))
            {
            }

            public override int GetHeuristic(Creature actor, out string reason)
            {
                reason = "LifeDrain";
                return 9; // 6 for damage and 3 for life drain bit
            }

            protected override void SideEffect(Creature actor, Creature target, Damage damage)
            {
                // The target must succeed on a DC 13 Constitution saving throw or its hit point maximum is reduced
                // by an amount equal to the damage taken. This reduction lasts until the target finishes a long rest.
                // The target dies if this effect reduces its hit point maximum to 0.
                if (!target.MakeSavingThrow(StatEnum.Constitution, 13, out _))
                {
                    Console.WriteLine($"// Wight Life Drain Side Effect");
                    target.DamageMaxHealth(damage.hits);
                }
            }
        }
    }
}