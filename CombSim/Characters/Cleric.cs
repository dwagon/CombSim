using System;
using System.Collections.Generic;
using CombSim.Gear;
using CombSim.Spells;

namespace CombSim.Characters
{
    public class Cleric : Character
    {
        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>
        {
            { 1, 9 }, { 2, -1 }, { 3, -1 }, { 4, -1 }
        };

        // CasterLevel: <SpellLevel: NumberOfSlots>
        private readonly Dictionary<int, Dictionary<int, int>> _spellsAtLevel =
            new Dictionary<int, Dictionary<int, int>>()
            {
                { 1, new Dictionary<int, int>() { { 1, 2 } } },
                { 2, new Dictionary<int, int>() { { 1, -1 } } },
                { 3, new Dictionary<int, int>() { { 1, -1 }, { 2, -1 } } },
            };

        public Cleric(string name, int level = 1, string team = "Clerics") : base(name, team)
        {
            Repr = "C";

            Level = level;
            MaxHitPoints = _hitPointsAtLevel[level];
            SpellCastingAbility = StatEnum.Wisdom;

            Stats.Add(StatEnum.Strength, new Stat(15));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(13));
            Stats.Add(StatEnum.Intelligence, new Stat(9));
            Stats.Add(StatEnum.Wisdom, new Stat(16));
            Stats.Add(StatEnum.Charisma, new Stat(11));
            AddEquipment(MeleeWeaponGear.Flail);
            AddEquipment(ArmourGear.Ring);
            AddEquipment(ArmourGear.Shield);
            AddEquipment(PotionsGear.HealingPotion);

            // Cantrips
            AddSpell(new SacredFlame());
            // AddSpell(new Guidance());
            // AddSpell(new SpareTheDying());

            // Level 1
            // AddSpell(new Bane());
            // AddSpell(new Bless());
            // AddSpell(new CureWounds());
            // AddSpell(new GuidingBolt());
            // AddSpell(new HealingWord());
            AddSpell(new InflictWounds());
        }

        public override string ToString()
        {
            var baseString = base.ToString();
            var spellString = "Spells: ";
            foreach (var kvp in _spellsAtLevel[Level])
            {
                spellString += $"L{kvp.Key} = {kvp.Value}; ";
            }

            return baseString + spellString;
        }

        public override bool CanCastSpell(Spell spell)
        {
            if (spell.Level == 0) return true;
            if (_spellsAtLevel[Level][spell.Level] >= 1) return true;
            return false;
        }

        public override void DoCastSpell(Spell spell)
        {
            Console.WriteLine($"// DoCastSpell(spell={spell.Name()}) Level: {spell.Level}");
            if (spell.Level == 0) return;
            _spellsAtLevel[Level][spell.Level]--;
        }
    }
}