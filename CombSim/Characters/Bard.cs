using System;
using System.Collections.Generic;
using CombSim.Gear;
using CombSim.Spells;
using Pastel;

namespace CombSim.Characters
{
    public class Bard : Character
    {
        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>
        {
            { 1, 9 }, { 2, 15 }, { 3, 21 }, { 4, 27 }, { 5, 33 }
        };

        // CasterLevel: <SpellLevel: NumberOfSlots>
        private readonly Dictionary<int, Dictionary<int, int>> _spellsAtLevel =
            new Dictionary<int, Dictionary<int, int>>
            {
                { 1, new Dictionary<int, int> { { 1, 2 } } },
                { 2, new Dictionary<int, int> { { 1, 3 } } },
                /*
                { 3, new Dictionary<int, int> { { 1, 4 }, { 2, 2 } } },
                { 4, new Dictionary<int, int> { { 1, 4 }, { 2, 3 } } },
                { 5, new Dictionary<int, int> { { 1, 4 }, { 2, 3 }, { 3, 2 } } }
                */
            };

        public Bard(string name, int level, string team) : base(name, level, team)
        {
            Repr = "B".Pastel(ConsoleColor.Red).PastelBg(ConsoleColor.DarkBlue);

            Level = level;
            MaxHitPoints = _hitPointsAtLevel[level];
            SpellCastingAbility = StatEnum.Charisma;

            Stats.Add(StatEnum.Strength, new Stat(10));
            Stats.Add(StatEnum.Dexterity, new Stat(16));
            Stats.Add(StatEnum.Constitution, new Stat(12));
            Stats.Add(StatEnum.Intelligence, new Stat(13));
            Stats.Add(StatEnum.Wisdom, new Stat(18));
            Stats.Add(StatEnum.Charisma, new Stat(16));
            AddEquipment(MeleeWeaponGear.Rapier);
            AddEquipment(ArmourGear.Leather);
            AddEquipment(new HealingPotion());

            AddSpell(new Thunderclap());
            // AddSpell(new ViciousMockery());

            // AddSpell(new CureWounds());
            // AddSpell(new EarthTremor());
            AddSpell(new HealingWord());
            // AddSpell(new HideousLaughter());
        }

        public override string ToString()
        {
            var baseString = base.ToString();
            var spellString = "; Spells: ";
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