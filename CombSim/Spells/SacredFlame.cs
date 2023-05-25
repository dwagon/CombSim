namespace CombSim.Spells
{
    public class SacredFlame : DcSaveSpell
    {
        public SacredFlame() : base("Sacred Flame", 0, ActionCategory.Action)
        {
            Reach = 60 / 5;
            DmgRoll = new DamageRoll("1d8", DamageTypeEnums.Radiant);
            SpellSavedEffect = SpellSavedEffect.NoDamage;
            SpellSaveAgainst = StatEnum.Dexterity;
        }
    }
}