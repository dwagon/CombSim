namespace CombSim.Spells
{
    public class FireBolt : ToHitSpell
    {
        public FireBolt() : base("Fire Bolt", 0, ActionCategory.Action)
        {
            Reach = 120 / 5;
            DmgRoll = new DamageRoll("1d10", DamageTypeEnums.Fire);
        }
    }
}