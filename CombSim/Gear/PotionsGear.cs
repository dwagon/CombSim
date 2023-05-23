namespace CombSim.Gear
{
    public static class PotionsGear
    {
        /*
         * Healing: 2d4+2
         * Greater Healing: 4d4+4
         * Superior Healing: 8d4+8
         * Supreme Healing: 10d4+20
         */

        public static readonly Potion HealingPotion = new GenericHealingPotion("Normal Healing Potion", "2d4+2");

        public static readonly Potion GreaterHealingPotion =
            new GenericHealingPotion("Greater Healing Potion", "4d4+4");

        public static readonly Potion SuperiorHealingPotion =
            new GenericHealingPotion("Superior Healing Potion", "8d4+8");

        public static readonly Potion SupremeHealingPotion =
            new GenericHealingPotion("Supreme Healing Potion", "10d4+20");

        private class GenericHealingPotion : Potion
        {
            private readonly string _healingAmount;

            public GenericHealingPotion(string name, string healing) : base(name)
            {
                _healingAmount = healing;
            }

            protected override void PotionEffect(Creature drinker)
            {
                var healed = Dice.Roll(_healingAmount);
                drinker.Heal(healed, Name);
            }

            protected override int GetHeuristic(Creature drinker, out string reason)
            {
                if (drinker.PercentHitPoints() > 50)
                {
                    reason = $"{drinker.Name} has greater than 50% hit points ({drinker.PercentHitPoints()}%)";
                    return 0;
                }

                var avgHealing = Dice.Roll(_healingAmount, max: true) / 2;
                if (drinker.HitPointsDown() > avgHealing)
                {
                    reason = $"{drinker.Name} is down more than {avgHealing}";
                    return 5;
                }

                var magicFormula = (100 - drinker.PercentHitPoints()) / 10 + 1;
                reason = $"{drinker.Name} is down {magicFormula}%";
                return magicFormula;
            }
        }
    }
}