namespace CombSim
{
    public class Damage
    {
        private int _damage;
        private DamageTypeEnums _type;

        public Damage(int damage, DamageTypeEnums type)
        {
            _damage = damage;
            _type = type;
        }
    }
}