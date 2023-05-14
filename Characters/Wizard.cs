using System.Collections.Generic;

namespace CombSim.Characters
{
    public class Wizard : Character
    {
        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>()
        {
            { 1, 8 }, { 2, 12 }
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

            AddSpell(new Spells.RayOfFrost());
        }
    }
}