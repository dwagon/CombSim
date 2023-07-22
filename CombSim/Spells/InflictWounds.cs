namespace CombSim.Spells
{
    public class InflictWounds : ToHitSpell
    {
        public InflictWounds(int castAtSpellLevel) : base("Inflict Wounds", 1, ActionCategory.Action)
        {
            Reach = 5 / 5;
            TouchSpell = true;
            DmgRoll = new DamageRoll($"{3 + castAtSpellLevel - 1}d10", DamageTypeEnums.Necrotic);
        }
    }
}