using CombSim.Gear;

namespace CombSim.Monsters
{
    public class Goblin : Monster
    {
        public Goblin(string name, string team = "Kobolds") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(8));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(10));
            Stats.Add(StatEnum.Intelligence, new Stat(10));
            Stats.Add(StatEnum.Wisdom, new Stat(8));
            Stats.Add(StatEnum.Charisma, new Stat(8));
            Repr = "g";
            HitDice = "2d6";
            AddEquipment(MeleeWeaponGear.Scimitar);
            AddEquipment(RangedWeaponGear.Shortbow);
            AddEquipment(ArmourGear.Leather);
            AddEquipment(ArmourGear.Shield);
        }
    }
}