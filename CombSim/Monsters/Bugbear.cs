// https://www.dndbeyond.com/monsters/bugbear

using CombSim.Gear;

namespace CombSim.Monsters
{
    public class Bugbear : Monster
    {
        private readonly RangedWeapon _javelin;

        public Bugbear(string name, string team = "Bugbears") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(15));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(13));
            Stats.Add(StatEnum.Intelligence, new Stat(8));
            Stats.Add(StatEnum.Wisdom, new Stat(11));
            Stats.Add(StatEnum.Charisma, new Stat(9));
            Repr = "B";
            HitDice = "5d8+5";
            ArmourClass(12);
            AddEquipment(ArmourGear.Hide);
            AddEquipment(ArmourGear.Shield);
            AddEquipment(new BruteMorningStar());
            _javelin = new Javelin();
            _javelin.AddAmmunition(3);
            AddEquipment(_javelin);
            ProficiencyBonus = 2;
        }

        public override string ToString()
        {
            var baseString = base.ToString();
            return baseString + $"Javelins: {_javelin.GetAmmunition()}";
        }

        private class BruteMorningStar : MeleeWeapon
        {
            public BruteMorningStar() : base("MorningStar", new DamageRoll("2d8", 0, DamageTypeEnums.Piercing))
            {
            }
        }
    }
}