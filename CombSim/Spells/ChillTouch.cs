namespace CombSim.Spells
{
    public class ChillTouch : ToHitSpell
    {
        public ChillTouch(int casterLevel) : base("Chill Touch", 0, ActionCategory.Action)
        {
            Reach = 120 / 5;
            if (casterLevel >= 11) DmgRoll = new DamageRoll("3d8", DamageTypeEnums.Necrotic);
            else if (casterLevel >= 5) DmgRoll = new DamageRoll("2d8", DamageTypeEnums.Necrotic);
            else DmgRoll = new DamageRoll("1d8", DamageTypeEnums.Necrotic);

            // TODO Undead disadvantage
            // TODO Can't heal
        }
    }
}