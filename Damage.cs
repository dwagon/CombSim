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

        public string ToString()
        {
            return $"{hits} {type}";
        }
    }
}