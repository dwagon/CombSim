// https://www.dndbeyond.com/monsters/gnoll

using System;
using CombSim.Gear;
using Pastel;

namespace CombSim.Monsters
{
    public class Gnoll : Monster
    {
        public Gnoll(string name, string team = "Ghouls") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(14));
            Stats.Add(StatEnum.Dexterity, new Stat(12));
            Stats.Add(StatEnum.Constitution, new Stat(11));
            Stats.Add(StatEnum.Intelligence, new Stat(6));
            Stats.Add(StatEnum.Wisdom, new Stat(10));
            Stats.Add(StatEnum.Charisma, new Stat(7));
            Repr = "G".Pastel(ConsoleColor.Green);
            HitDice = "5d8";
            AddEquipment(ArmourGear.Hide);
            AddEquipment(ArmourGear.Shield);
            AddEquipment(MeleeWeaponGear.Spear);
            AddEquipment(RangedWeaponGear.Longbow);
            AddAction(new GnollBite());

            ProficiencyBonus = 2;
            OnAnyBeingKilled += Rampage;
        }

        // When the gnoll reduces a creature to 0 hit points with a melee attack on its turn,
        // the gnoll can take a bonus action to move up to half its speed and make a bite attack.
        private void Rampage(object sender, OnAnyBeingKilledEventArgs e)
        {
            if (e.Source == this && e.Action is MeleeAttack)
            {
                var rampageAction = new GnollRampageBite();
                Moves = Speed / 2;
                AddAction(rampageAction);
                rampageAction.Perform(this);
                RemoveAction(rampageAction);
            }
        }

        private class GnollBite : MeleeAttack
        {
            public GnollBite() : base("Bite", new DamageRoll("1d4", DamageTypeEnums.Piercing))
            {
            }
        }

        private class GnollRampageBite : Action
        {
            public GnollRampageBite() : base("Bite", ActionCategory.Bonus)
            {
            }

            public override void DoAction(Creature actor)
            {
                Console.WriteLine($"// RampageBite (actor={actor.Name})");
                var bite = actor.PickActionByName("Bite");
                bite.Perform(actor);
            }
        }
    }
}