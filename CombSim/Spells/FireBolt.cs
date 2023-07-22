namespace CombSim.Spells
{
    public class FireBolt : ToHitSpell
    {
        public FireBolt(int casterLevel) : base("Fire Bolt", 0, ActionCategory.Action)
        {
            Reach = 120 / 5;
            if (casterLevel >= 11) DmgRoll = new DamageRoll("3d10", DamageTypeEnums.Fire);
            else if (casterLevel >= 5) DmgRoll = new DamageRoll("2d10", DamageTypeEnums.Fire);
            else DmgRoll = new DamageRoll("1d10", DamageTypeEnums.Fire);
        }
    }
}