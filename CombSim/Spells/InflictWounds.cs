namespace CombSim.Spells
{
    public class InflictWounds : ToHitSpell
    {
        public InflictWounds() : base("Inflict Wounds", 1, ActionCategory.Action)
        {
            Reach = 5 / 5;
            TouchSpell = true;
            DmgRoll = new DamageRoll("3d10", DamageTypeEnums.Necrotic);
        }
    }
}