using CombSim.Gear;

namespace CombSim.Monsters
{
    public class Ogre : Monster
    {
        public Ogre(string name, string team = "Ogres") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(19));
            Stats.Add(StatEnum.Dexterity, new Stat(8));
            Stats.Add(StatEnum.Constitution, new Stat(16));
            Stats.Add(StatEnum.Intelligence, new Stat(5));
            Stats.Add(StatEnum.Wisdom, new Stat(7));
            Stats.Add(StatEnum.Charisma, new Stat(7));
            Repr = "o";
            ArmourClass = 12;
            HitDice = "7d10+21";
            AddEquipment(MeleeWeaponGear.Greatclub);
            AddEquipment(RangedWeaponGear.Javelin);
            AddEquipment(ArmourGear.Hide);
        }
    }
}