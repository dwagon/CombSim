namespace CombSim.Spells
{
    public class PoisonSpray : DcSaveSpell
    {
        public PoisonSpray(int casterLevel) : base("Poison Spray", 0, ActionCategory.Action)
        {
            if (casterLevel >= 11) DmgRoll = new DamageRoll("3d12", DamageTypeEnums.Poison);
            else if (casterLevel >= 5) DmgRoll = new DamageRoll("2d12", DamageTypeEnums.Poison);
            else DmgRoll = new DamageRoll("1d12", DamageTypeEnums.Poison);
            SpellSavedEffect = SpellSavedEffect.NoDamage;
            SpellSaveAgainst = StatEnum.Constitution;
            Reach = 5 / 5;
        }
    }
}