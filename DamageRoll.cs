namespace CombSim
{
    public class DamageRoll
    {
        private readonly string _roll;
        private readonly DamageTypeEnums _type;

        public DamageRoll(string roll, int bonus, DamageTypeEnums type)
        {
            _roll = roll;
            _type = type;
        }
        
        public DamageRoll(string roll, DamageTypeEnums type)
        {
            _roll = roll;
            _type = type;
        }

        public Damage Roll(bool max = false)
        {
            var dmg = 0;
            dmg += Dice.Roll(_roll, max);
            return new Damage(dmg, _type);
        }

        public static DamageRoll operator +(DamageRoll dmgRoll, int bonus)
        {
            return new DamageRoll(dmgRoll._roll, dmgRoll._type);
        }
    }
}