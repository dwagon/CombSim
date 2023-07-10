namespace CombSim.Spells
{
    public class ViciousMockery : DcSaveSpell
    {
        public ViciousMockery() : base("Vicious Mockery", 0, ActionCategory.Action)
        {
            DmgRoll = new DamageRoll("1d4", DamageTypeEnums.Psychic);
            SpellSavedEffect = SpellSavedEffect.NoDamage;
            SpellSaveAgainst = StatEnum.Wisdom;
            Reach = 60 / 5;
            // TODO: Disadvantage on next attack roll
        }

        protected override void SideEffect(Creature actor, Creature target, Damage damage)
        {
            target.AddEffect(new Mocked());
            target.OnTurnEnd += TurnEnd;
        }

        private void TurnEnd(object sender, Creature.OnTurnEndEventArgs e)
        {
            var eff = e.Creature.GetEffectByName("Mocked");
            e.Creature.OnTurnEnd -= TurnEnd;
            if (eff != null) // Effect can also be removed by use
            {
                e.Creature.RemoveEffect(eff);
            }
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var normal = base.GetHeuristic(actor, out reason);
            if (normal > 0)
            {
                return normal + 3; // 3 for disadvantage
            }

            return normal;
        }

        private class Mocked : Effect
        {
            public Mocked() : base("Mocked")
            {
            }

            public override bool HasAdvantageAgainstMe(Creature target, Creature attacker)
            {
                target.RemoveEffect(this);
                return true;
            }
        }
    }
}