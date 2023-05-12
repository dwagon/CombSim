namespace CombSim
{
    public class Damage
    {
        public int hits;
        public DamageTypeEnums type;

        public Damage(int hits, DamageTypeEnums type)
        {
            this.hits = hits;
            this.type = type;
        }

        public override string ToString()
        {
            return $"{hits} {type}";
        }
        
        public static Damage operator *(Damage damage, int multiplier)
        {
            return new Damage(damage.hits * multiplier, damage.type);
        }
        
        public static Damage operator /(Damage damage, int divisor)
        {
            return new Damage(damage.hits / divisor, damage.type);
        }
    }
}