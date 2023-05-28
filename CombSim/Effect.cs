namespace CombSim
{
    public class Effect
    {
        public Effect(string name, bool hidden = false)
        {
            Name = name;
            Hidden = hidden;
        }

        public bool Hidden { get; protected set; }

        public string Name { get; protected set; }

        // Override for when the effect starts
        public virtual void Start(Creature target)
        {
        }

        // Override for when the effect ends
        public virtual void End(Creature target)
        {
        }

        public virtual bool HasAdvantageAgainstMe(Creature target, Creature attacker)
        {
            return false;
        }

        public virtual bool HasDisadvantageAgainstMe(Creature actor, Creature victim)
        {
            return false;
        }

        public virtual void DoAttack(Attack attackAction, Creature actor, Creature victim)
        {
        }
    }
}