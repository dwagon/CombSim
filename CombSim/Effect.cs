namespace CombSim
{
    public class Effect
    {
        protected string Name;

        public Effect(string name)
        {
            Name = name;
        }

        // Override for when the effect starts
        public virtual void Start(Creature target)
        {
        }

        // Override for when the effect ends
        public virtual void End(Creature target)
        {
        }
    }
}