namespace CombSim
{

    public class RangedAttack : Attack
    {
        private int _srange;
        private int _lrange;
        
        public RangedAttack(string name, DamageRoll damageroll, int srange, int lrange) : base(name, damageroll)
        {
            _srange = srange;
            _lrange = lrange;
        }
    }
}