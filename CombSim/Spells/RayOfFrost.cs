namespace CombSim.Spells
{
    public class RayOfFrost : ToHitSpell
    {
        public RayOfFrost(int casterLevel) : base("Ray of Frost", 0, ActionCategory.Action)
        {
            Reach = 60 / 5;
            if (casterLevel >= 11) DmgRoll = new DamageRoll("3d8", DamageTypeEnums.Cold);
            else if (casterLevel >= 5) DmgRoll = new DamageRoll("2d8", DamageTypeEnums.Cold);
            else DmgRoll = new DamageRoll("1d8", DamageTypeEnums.Cold);
        }
    }
}