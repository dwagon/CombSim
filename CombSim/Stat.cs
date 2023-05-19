namespace CombSim
{
    public enum StatEnum
    {
        Strength,
        Intelligence,
        Dexterity,
        Wisdom,
        Constitution,
        Charisma
    }

    public class Stat
    {
        private readonly int _value;

        public Stat(int value)
        {
            _value = value;
        }

        public int Bonus()
        {
            return (_value - 10) / 2;
        }

        public int Roll(bool hasAdvantage = false, bool hasDisadvantage = false)
        {
            return Dice.RollD20(hasAdvantage, hasDisadvantage) + Bonus();
        }
    }
}