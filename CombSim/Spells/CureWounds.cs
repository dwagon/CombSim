namespace CombSim.Spells
{
    public class CureWounds : HealingSpell
    {
        public CureWounds() : base("Cure Wounds", 1, ActionCategory.Action)
        {
            Reach = 5 / 5;
            HealingRoll = new DamageRoll("1d8", DamageTypeEnums.None);
            TouchSpell = true;
        }

        protected override Damage HealingAmount(Creature actor, Creature target, bool max = false)
        {
            var hp = HealingRoll.Roll(max) + actor.HealingBonus() + actor.SpellModifier();
            return hp;
        }
    }
}