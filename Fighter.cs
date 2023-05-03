namespace CombSim
{
    public class Fighter: Character
    {
        public Fighter(string name) : base(name)
        {
            _repr = "F";
            HitPoints = 12;
            Level = 1;
            
            Stats.Add(StatEnum.Strength, new Stat(16));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(15));
            Stats.Add(StatEnum.Intelligence, new Stat(11));
            Stats.Add(StatEnum.Wisdom, new Stat(13));
            Stats.Add(StatEnum.Charisma, new Stat(9));
        }
    }
}