using System;
using System.Collections.Generic;
using CombSim.Gear;
using CombSim.Spells;
using Pastel;

namespace CombSim.Characters
{
    public class Warlock : Character
    {
        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>
        {
            { 1, 10 }, { 2, 18 }, { 3, 24 }, { 4, 31 }, { 5, 38 }
        };

        // CasterLevel: <SpellLevel: NumberOfSlots>
        private readonly Dictionary<int, int> _spellsAtLevel =
            new Dictionary<int, int>
            {
                { 1, 1 },
                { 2, 2 },
                { 3, 2 },
                { 4, 2 },
                { 5, 2 },
            };

        public Warlock(string name, int level = 1, string team = "Warlocks") : base(name, level, team)
        {
            Repr = "W".Pastel(ConsoleColor.Red).PastelBg(ConsoleColor.DarkBlue);

            Level = level;
            MaxHitPoints = _hitPointsAtLevel[level];
            SpellCastingAbility = StatEnum.Charisma;

            Stats.Add(StatEnum.Strength, new Stat(8));
            Stats.Add(StatEnum.Dexterity, new Stat(15));
            Stats.Add(StatEnum.Constitution, new Stat(14));
            Stats.Add(StatEnum.Intelligence, new Stat(13));
            Stats.Add(StatEnum.Wisdom, new Stat(10));
            Stats.Add(StatEnum.Charisma, new Stat(15));

            AddEquipment(new HealingPotion());
            AddEquipment(MeleeWeaponGear.Quarterstaff);

            // Cantrips
            AddSpell(new Thunderclap());

            // Level 1
            AddSpell(new BurningHands());
            // AddSpell(new HellishRebuke());

            int ebRange = 120 / 5;
            int ebDmgBonus = 0;
            if (Level >= 2)
            {
                Attributes.Add(Attribute.AgonizingBlast);
                Attributes.Add(Attribute.EldritchSpear);
            }

            if (Level >= 3) // Pact of the Tome
            {
                AddSpell(new FireBolt());
                AddSpell(new ShockingGrasp());
                // AddSpell(new ViciousMockery());
                AddEquipment(ArmourGear.StuddedLeatherPlusOne);
            }
            else
            {
                AddEquipment(ArmourGear.StuddedLeather);
            }


            if (level >= 4)
            {
                Stats[StatEnum.Charisma] = new Stat(18);
                AddEquipment(new CloakOfDisplacement());
            }

            if (level >= 5)
            {
                AddSpell(new Fireball());
                AddEquipment(new GreaterHealingPotion());
            }

            if (HasAttribute(Attribute.EldritchSpear)) ebRange = 300 / 5;
            if (HasAttribute(Attribute.AgonizingBlast)) ebDmgBonus = Stats[StatEnum.Charisma].Bonus();
            AddSpell(new EldritchBlast(ebRange, ebDmgBonus));
        }

        public override string ToString()
        {
            var baseString = base.ToString();
            var spellString = $"; Spells: {_spellsAtLevel[Level]}";
            return baseString + spellString;
        }

        public override bool CanCastSpell(Spell spell)
        {
            if (spell.Level == 0) return true;
            if (_spellsAtLevel[Level] >= 1) return true;
            return false;
        }

        public override void DoCastSpell(Spell spell)
        {
            if (spell.Level == 0) return;
            _spellsAtLevel[Level]--;
        }
    }
}