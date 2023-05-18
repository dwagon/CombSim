namespace CombSim.Spells
{
    public class RayOfFrost : ToHitSpell
    {
        public RayOfFrost() : base("Ray of Frost", 0, ActionCategory.Action)
        {
            Reach = 60 / 5;
            DmgRoll = new DamageRoll("1d8", DamageTypeEnums.Cold);
        }
    }
}