using System;
using System.Collections.Generic;
using CombSim.Spells;

namespace CombSim.Characters
{
    public class Wizard : Character
    {
        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>()
        {
            { 1, 8 }, { 2, 14 }, {3, 20}
        };
        
        // CasterLevel: <SpellLevel: NumberOfSlots>
        private readonly Dictionary<int, Dictionary<int, int>> _spellsAtLevel = new Dictionary<int, Dictionary<int, int>>()
        {
            {1, new Dictionary<int, int>() {{1, 2}}},
            {2, new Dictionary<int, int>() {{1, 3}}},
            {3, new Dictionary<int, int>() {{1, 4}, {2, 2}}},

        };

        public Wizard(string name, int level = 1, string team = "Wizards") : base(name, team)
        {
            Repr = "W";
            SpellCastingAbility = StatEnum.Intelligence;
            Level = level;
            MaxHitPoints = _hitPointsAtLevel[level];

            Stats.Add(StatEnum.Strength, new Stat(9));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(15));
            Stats.Add(StatEnum.Intelligence, new Stat(15));
            Stats.Add(StatEnum.Wisdom, new Stat(11));
            Stats.Add(StatEnum.Charisma, new Stat(13));

            AddEquipment(Gear.Quarterstaff);
            // AddEquipment(Gear.LightCrossbow);

            AddSpell(new RayOfFrost());
            AddSpell(new MagicMissile());
            AddSpell(new PoisonSpray());
            if (Level >= 3)
            {
                AddSpell(new ScorchingRay());
            }
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
            Console.WriteLine($"// DoCastSpell(spell={spell.Name()}) {spell.Level}");
            if (spell.Level == 0) return;
            _spellsAtLevel[Level][spell.Level]--;
        }
    }
}