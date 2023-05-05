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
        private HashSet<ConditionEnum> _conditions = new HashSet<ConditionEnum>();

        public void SetCondition(ConditionEnum cond)
        {
            _conditions.Add(cond);
        }

        public bool HasCondition(ConditionEnum cond)
        {
            return _conditions.Contains(cond);  
        }
    }
}