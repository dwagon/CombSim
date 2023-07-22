namespace CombSim.Spells
{
    public class HealingWord : HealingSpell
    {
        public HealingWord(int castAtSpellLevel) : base("Healing Word", 1, ActionCategory.Bonus)
        {
            Reach = 60 / 5;
            HealingRoll = new DamageRoll($"{castAtSpellLevel}d4", DamageTypeEnums.None);
        }

        protected override Damage HealingAmount(Creature actor, Creature target, bool max = false)
        {
            var hp = HealingRoll.Roll(max) + actor.HealingBonus() + actor.SpellModifier();
            return hp;
        }
    }
}