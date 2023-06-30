namespace CombSim
{
    public class Damage
    {
        public Damage(int hits, DamageTypeEnums type)
        {
            this.hits = hits;
            this.type = type;
        }

        public int hits { get; }
        public DamageTypeEnums type { get; }

        public override string ToString()
        {
            return $"{hits} {type}";
        }

        public static Damage operator *(Damage damage, int multiplier)
        {
            return new Damage(damage.hits * multiplier, damage.type);
        }

        public static Damage operator +(Damage damage1, Damage damage2)
        {
            return new Damage(damage1.hits + damage2.hits, damage1.type);
        }

        public static Damage operator +(Damage damage1, int dmgBonus)
        {
            return new Damage(damage1.hits + dmgBonus, damage1.type);
        }

        public static Damage operator /(Damage damage, int divisor)
        {
            return new Damage(damage.hits / divisor, damage.type);
        }
    }
}