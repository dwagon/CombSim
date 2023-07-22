namespace CombSim.Spells
{
    public class SacredFlame : DcSaveSpell
    {
        public SacredFlame(int casterLevel) : base("Sacred Flame", 0, ActionCategory.Action)
        {
            Reach = 60 / 5;
            if (casterLevel >= 11) DmgRoll = new DamageRoll("3d8", DamageTypeEnums.Radiant);
            else if (casterLevel >= 5) DmgRoll = new DamageRoll("2d8", DamageTypeEnums.Radiant);
            else DmgRoll = new DamageRoll("1d8", DamageTypeEnums.Radiant);
            SpellSavedEffect = SpellSavedEffect.NoDamage;
            SpellSaveAgainst = StatEnum.Dexterity;
        }
    }
}