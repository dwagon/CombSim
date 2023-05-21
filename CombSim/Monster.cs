using System;

namespace CombSim
{
    public abstract class Monster : Creature
    {
        protected Monster(string name, string team = "Monsters") : base(name, team)
        {
        }

        protected string HitDice { get; set; }

        public override void Initialise()
        {
            MaxHitPoints = RollHitDice(HitDice);
            base.Initialise();
        }

        protected override void FallenUnconscious()
        {
            HitPoints = 0;
            Died();
        }

        private int RollHitDice(string hitDice)
        {
            return Math.Max(Dice.Roll(hitDice), 1);
        }
    }
}