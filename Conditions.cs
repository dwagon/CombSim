using System.Collections.Generic;

namespace CombSim
{
    public enum ConditionEnum
    {
        Blinded,
        Charmed,
        Dead,
        Deafened,
        Exhaustion,
        Frightened,
        Grappled,
        Incapacitated,
        Invisible,
        Ok,
        Paralyzed,
        Petrified,
        Poisoned,
        Prone,
        Restrained,
        Stunned,
        Unconscious
    }
    
    public class Conditions
    {
        private readonly HashSet<ConditionEnum> _conditions = new HashSet<ConditionEnum>();

        public void SetCondition(ConditionEnum condition)
        {
            _conditions.Add(condition);
        }

        public bool HasCondition(ConditionEnum condition)
        {
            return _conditions.Contains(condition);  
        }

        public void RemoveCondition(ConditionEnum condition)
        {
            _conditions.Remove(condition);
        }

        public override string ToString()
        {
            return string.Join(", ", _conditions);
        }
    }
}