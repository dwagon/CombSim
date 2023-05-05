namespace CombSim
{
    public class DamageRoll
    {
        private string _roll;
        private int _bonus;
        private DamageTypeEnums _type;
        
        public DamageRoll(string roll, int bonus, DamageTypeEnums type)
        {
            _roll = roll;
            _bonus = bonus;
            _type = type;
        }

        public Damage Roll()
        {
            int dmg = 0;
            dmg += Dice.Roll(_roll);
            dmg += _bonus;
            return new Damage(dmg, _type);
        }
    }
}