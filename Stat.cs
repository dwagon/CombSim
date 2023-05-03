namespace CombSim
{
    public class Stat
    {
        private int _value;

        public Stat(int value)
        {
            this._value = value;
        }
        
        public int Bonus()
        {
            return (_value - 10) / 2;
        }

        public int Roll()
        {
            return Dice.RollD20() + Bonus();
        }
    }
}