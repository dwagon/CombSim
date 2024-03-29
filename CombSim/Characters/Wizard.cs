using System;
using System.Collections.Generic;
using CombSim.Gear;
using CombSim.Spells;
using Pastel;

namespace CombSim.Characters
{
    public class Wizard : Character
    {
        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>()
        {
            { 1, 8 }, { 2, 14 }, { 3, 20 }, { 4, 26 }, { 5, 32 }, { 6, 38 }
        };

        // CasterLevel: <SpellLevel: NumberOfSlots>
        private readonly Dictionary<int, Dictionary<int, int>> _spellsAtLevel =
            new Dictionary<int, Dictionary<int, int>>()
            {
                { 1, new Dictionary<int, int> { { 1, 2 } } },
                { 2, new Dictionary<int, int> { { 1, 3 } } },
                { 3, new Dictionary<int, int> { { 1, 4 }, { 2, 2 } } },
                { 4, new Dictionary<int, int> { { 1, 4 }, { 2, 3 } } },
                { 5, new Dictionary<int, int> { { 1, 4 }, { 2, 3 }, { 3, 2 } } },
                { 6, new Dictionary<int, int> { { 1, 4 }, { 2, 3 }, { 3, 3 } } }
            };

        public Wizard(string name, int level = 1, string team = "Wizards") : base(name, level, team)
        {
            Repr = "W".Pastel(ConsoleColor.Magenta).PastelBg(ConsoleColor.DarkBlue);
            SpellCastingAbility = StatEnum.Intelligence;
            Level = level;
            MaxHitPoints = _hitPointsAtLevel[level];

            Stats.Add(StatEnum.Strength, new Stat(9));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(15));
            Stats.Add(StatEnum.Intelligence, new Stat(16));
            Stats.Add(StatEnum.Wisdom, new Stat(11));
            Stats.Add(StatEnum.Charisma, new Stat(13));

            AddEquipment(MeleeWeaponGear.Quarterstaff);
            AddEquipment(new SuperiorHealingPotion());

            AddSpell(new RayOfFrost(Level));
            AddSpell(new BurningHands(Level));
            AddSpell(new MagicMissile(Level));
            AddSpell(new PoisonSpray(Level));
            if (Level >= 3)
            {
                AddSpell(new ScorchingRay(Level));
                AddEquipment(new RingOfProtection());
            }

            if (level >= 4)
            {
                Stats[StatEnum.Intelligence] = new Stat(18);
                AddEquipment(new BracersOfDefence());
            }

            if (level >= 5)
            {
                AddSpell(new Fireball(Level));
                AddEquipment(new GreaterHealingPotion());
            }
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
            if (spell.Level == 0) return;
            _spellsAtLevel[Level][spell.Level]--;
        }
    }
}