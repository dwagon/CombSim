namespace CombSim.Gear
{
    /*
     * Healing: 2d4+2
     * Greater Healing: 4d4+4
     * Superior Healing: 8d4+8
     * Supreme Healing: 10d4+20
     */

    public class GenericHealingPotion : Potion
    {
        private readonly string _healingAmount;

        protected GenericHealingPotion(string name, string healing) : base(name)
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
            var magic = (int)(5f * drinker.HitPointsDown() / avgHealing);
            if (avgHealing > drinker.HitPointsDown()) magic /= 2;

            reason =
                $"{drinker.Name} down {drinker.HitPointsDown()} HP. {Name} can cure on average {avgHealing} HP";
            return magic;
        }
    }

    public class HealingPotion : GenericHealingPotion
    {
        public HealingPotion() : base("Healing Potion", "2d4+2")
        {
        }
    }

    public class GreaterHealingPotion : GenericHealingPotion
    {
        public GreaterHealingPotion() : base("Greater Healing Potion", "4d4+4")
        {
        }
    }

    public class SuperiorHealingPotion : GenericHealingPotion
    {
        public SuperiorHealingPotion() : base("Superior Healing Potion", "8d4+8")
        {
        }
    }

    public class SupremeHealingPotion : GenericHealingPotion
    {
        public SupremeHealingPotion() : base("Supreme Healing Potion", "8d4+8")
        {
        }
    }
}