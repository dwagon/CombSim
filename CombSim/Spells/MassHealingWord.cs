namespace CombSim.Spells
{
    public class MassHealingWord : HealingSpell
    {
        public MassHealingWord(int castAtSpellLevel) : base("Mass Healing Word", 3, ActionCategory.Bonus)
        {
            Reach = 60 / 5;
            HealingRoll = new DamageRoll($"{1 + castAtSpellLevel - 3}d4", DamageTypeEnums.None);
            NumRecipients = 6;
        }

        protected override Damage HealingAmount(Creature actor, Creature target, bool max = false)
        {
            var hp = HealingRoll.Roll(max) + actor.HealingBonus() + actor.SpellModifier();
            return hp;
        }
    }
}