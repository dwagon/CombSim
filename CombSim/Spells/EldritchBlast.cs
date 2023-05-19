namespace CombSim.Spells
{
    public class EldritchBlast: ToHitSpell
    {
        public EldritchBlast() : base("Eldritch Blast", 0, ActionCategory.Action)
        {
            Reach = 120 / 5;
            DmgRoll = new DamageRoll("1d10", DamageTypeEnums.Force);
        }
    }
}