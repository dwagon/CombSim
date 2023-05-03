using System.Collections.Generic;

namespace CombSim
{
    public abstract class Creature
    {
        public string Name { get; private set; }
        public int HitPoints { get; protected set; }
        protected Dictionary<StatEnum, Stat> Stats;
        protected string _repr;

        protected Creature(string name)
        {
            this.Name = name;
            Stats = new Dictionary<StatEnum, Stat>();
        }

        public string GetRepr()
        {
            return _repr;
        }

        public int RollInitiative()
        {
            return Stats[StatEnum.Dexterity].Roll();
        }

        public void TakeTurn()
        {
            
        }
    }
}