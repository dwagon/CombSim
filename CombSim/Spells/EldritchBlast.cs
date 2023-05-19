namespace CombSim.Spells
{
    public class EldritchBlast : ToHitSpell
    {
        public EldritchBlast(int reach = 120 / 5, int dmgBonus = 0) : base("Eldritch Blast", 0, ActionCategory.Action)
        {
            Reach = reach;
            DmgRoll = new DamageRoll("1d10", dmgBonus, DamageTypeEnums.Force);
        }
    }
}