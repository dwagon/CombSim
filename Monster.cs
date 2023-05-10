using System;

namespace CombSim
{
    public abstract class Monster : Creature
    {
        protected Monster(string name, string team = "Monsters") : base(name, team)
        {
        }

        public string HitDice { get; protected set; }

        public override void Initialise()
        {
            MaxHitPoints = RollHitDice(HitDice);
            base.Initialise();
        }

        protected override void FallenUnconscious()
        {
            NarrationLog.LogMessage($"{Name} has died");
            Conditions.SetCondition(ConditionEnum.Dead);
            Conditions.RemoveCondition(ConditionEnum.Ok);
            HitPoints = 0;
        }

        private int RollHitDice(string hitDice)
        {
            return Math.Max(Dice.Roll(hitDice), 1);
        }
    }
}