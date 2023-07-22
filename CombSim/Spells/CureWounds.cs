namespace CombSim.Spells
{
    public class CureWounds : HealingSpell
    {
        public CureWounds(int castAtSpellLevel) : base("Cure Wounds", 1, ActionCategory.Action)
        {
            Reach = 5 / 5;
            HealingRoll = new DamageRoll($"{castAtSpellLevel}d8", DamageTypeEnums.None);
            TouchSpell = true;
        }

        protected override Damage HealingAmount(Creature actor, Creature target, bool max = false)
        {
            var hp = HealingRoll.Roll(max) + actor.HealingBonus() + actor.SpellModifier();
            return hp;
        }
    }
}