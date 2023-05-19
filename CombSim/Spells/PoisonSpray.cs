namespace CombSim.Spells
{
    public class PoisonSpray: DcSaveSpell
    {
        public PoisonSpray() : base("Poison Spray", 0, ActionCategory.Action)
        {
            DmgRoll = new DamageRoll("1d12", DamageTypeEnums.Poison);
            SpellSavedEffect = SpellSavedEffect.NoDamage;
            SpellSaveAgainst = StatEnum.Constitution;
            Reach = 5 / 5;
        }
    }
}