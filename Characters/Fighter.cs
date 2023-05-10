using System;

namespace CombSim
{
    public class Fighter : Character
    {
        public Fighter(string name, string team = "Fighters") : base(name, team)
        {
            Repr = "F";
            MaxHitPoints = 12;
            Level = 1;

            Stats.Add(StatEnum.Strength, new Stat(16));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(15));
            Stats.Add(StatEnum.Intelligence, new Stat(11));
            Stats.Add(StatEnum.Wisdom, new Stat(13));
            Stats.Add(StatEnum.Charisma, new Stat(9));
            AddEquipment(Gear.Longsword);
            AddEquipment(Gear.Plate);
            // AddEquipment(Gear.Shield);
            Console.WriteLine($"{name} has an AC of {ArmourClass}");
        }
    }
}