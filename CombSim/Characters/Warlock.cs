using System.Collections.Generic;
using CombSim.Gear;
using CombSim.Spells;

namespace CombSim.Characters
{
    public class Warlock : Character
    {
        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>()
        {
            { 1, 10 },
        };
        
        // CasterLevel: <SpellLevel: NumberOfSlots>
        private readonly Dictionary<int, Dictionary<int, int>> _spellsAtLevel = new Dictionary<int, Dictionary<int, int>>()
        {
            {1, new Dictionary<int, int>() {{1, 1}}},
        };

        public Warlock(string name, int level = 1, string team = "Warlocks") : base(name, team)
        {
            Repr = "w";

            Level = level;
            MaxHitPoints = _hitPointsAtLevel[level];
            SpellCastingAbility = StatEnum.Charisma;

            Stats.Add(StatEnum.Strength, new Stat(8));
            Stats.Add(StatEnum.Dexterity, new Stat(15));
            Stats.Add(StatEnum.Constitution, new Stat(14));
            Stats.Add(StatEnum.Intelligence, new Stat(13));
            Stats.Add(StatEnum.Wisdom, new Stat(10));
            Stats.Add(StatEnum.Charisma, new Stat(15));
 
            AddEquipment(PotionsGear.HealingPotion);
            AddEquipment(ArmourGear.StuddedLeather);
            AddEquipment(MeleeWeaponGear.Quarterstaff);

            // Cantrips
            AddSpell(new Thunderclap());
            AddSpell(new EldritchBlast());
            
            // Level 1
            AddSpell(new BurningHands());
            // AddSpell(new HellishRebuke());
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
            if (spell.Level == 0) return;
            _spellsAtLevel[Level][spell.Level]--;
        }
    }
}