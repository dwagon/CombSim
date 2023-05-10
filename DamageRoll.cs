namespace CombSim
{
    public class DamageRoll
    {
        private readonly int _bonus;
        private readonly string _roll;
        private readonly DamageTypeEnums _type;

        public DamageRoll(string roll, int bonus, DamageTypeEnums type)
        {
            _roll = roll;
            _bonus = bonus;
            _type = type;
        }

        public Damage Roll(bool max = false)
        {
            var dmg = 0;
            dmg += Dice.Roll(_roll, max);
            dmg += _bonus;
            return new Damage(dmg, _type);
        }
    }
}