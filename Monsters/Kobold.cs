using System.ComponentModel;

namespace CombSim
{
    public class Kobold: Monster
    {
        public Kobold(string name, string team="Kobolds") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(7));
            Stats.Add(StatEnum.Dexterity, new Stat(15));
            Stats.Add(StatEnum.Constitution, new Stat(9));
            Stats.Add(StatEnum.Intelligence, new Stat(8));
            Stats.Add(StatEnum.Wisdom, new Stat(7));
            Stats.Add(StatEnum.Charisma, new Stat(8));
            Repr = "k";
            ArmourClass = 12;
            HitDice = "2d6-2";
            AddEquipment(Gear.Dagger);
            AddEquipment(Gear.Sling);
            Initialize();
        }
    }
}