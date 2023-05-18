namespace CombSim
{
    public class DamageRoll
    {
        private readonly string _roll;
        private readonly int _bonus;
        private readonly DamageTypeEnums _type;

        public DamageRoll(string roll, int bonus=0, DamageTypeEnums type=DamageTypeEnums.None)
        {
            _roll = roll;
            _bonus = bonus;
            _type = type;
        }
        
        public DamageRoll(string roll, DamageTypeEnums type)
        {
            _roll = roll;
            _type = type;
        }

        public override string ToString()
        {
            if(_bonus != 0)
                return $"{_roll} + {_bonus}";
            return $"{_roll}";
        }

        public Damage Roll(bool max = false)
        {
            var dmg = 0;
            dmg += Dice.Roll(_roll, max);
            return new Damage(dmg, _type) + _bonus;
        }

        public static DamageRoll operator +(DamageRoll dmgRoll, int bonus)
        {
            return new DamageRoll(dmgRoll._roll, dmgRoll._bonus+bonus, dmgRoll._type);
        }
    }
}