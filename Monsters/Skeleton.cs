namespace CombSim.Monsters
{
    public class Skeleton : Monster
    {
        public Skeleton(string name, string team = "Skeletons") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(10));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(15));
            Stats.Add(StatEnum.Intelligence, new Stat(6));
            Stats.Add(StatEnum.Wisdom, new Stat(8));
            Stats.Add(StatEnum.Charisma, new Stat(5));
            Repr = "s";
            ArmourClass = 13;
            HitDice = "2d8+4";
            AddEquipment(Gear.Shortsword);
            AddEquipment(Gear.Shortbow);
            Vulnerable.Add(DamageTypeEnums.Bludgeoning);
            Immune.Add(DamageTypeEnums.Poison);
        }
    }
}