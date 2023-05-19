namespace CombSim.Spells
{
    public class ChillTouch : ToHitSpell
    {
        public ChillTouch() : base("Chill Touch", 0, ActionCategory.Action)
        {
            Reach = 120 / 5;
            DmgRoll = new DamageRoll("1d8", DamageTypeEnums.Necrotic);
            
            // TODO Undead disadvantage
            // TODO Can't heal
        }
    }
}