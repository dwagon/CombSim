using System;

namespace CombSim
{
    public abstract class Monster: Creature
    {
        public string HitDice { get; protected set; }
        
        protected Monster(string name, string team="Monsters") : base(name, team)
        {
        }

        protected void Initialize()
        {
            HitPoints = RollHitDice(HitDice);
        }

        private int RollHitDice(string hitDice)
        {
            return Math.Max(Dice.Roll(hitDice), 1);
        }

        public string GetRepr()
        {
            return Name + " HP: " + HitPoints;
        }
    }
}