namespace CombSim.Spells
{
    public class HealingWord : HealingSpell
    {
        public HealingWord() : base("Healing Word", 1, ActionCategory.Bonus)
        {
            Reach = 60 / 5;
            HealingRoll = new DamageRoll("1d4", DamageTypeEnums.None);
        }
    }
}