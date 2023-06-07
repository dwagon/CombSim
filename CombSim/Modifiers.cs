using System;
using System.Collections.Generic;

namespace CombSim
{
    public class Modifiers
    {
        private readonly List<IModifier> _allModifiers;

        public Modifiers()
        {
            _allModifiers = new List<IModifier>();
        }

        public void Add(IModifier modifier)
        {
            _allModifiers.Add(modifier);
        }

        public void Remove(IModifier modifier)
        {
            _allModifiers.Remove(modifier);
        }

        public int ArmourModification(Attack attack)
        {
            var result = 0;
            foreach (var modifier in _allModifiers)
                result += modifier.ArmourModification(attack);
            return result;
        }

        public int SavingThrowModification(StatEnum stat)
        {
            var result = 0;
            foreach (var modifier in _allModifiers)
                result += modifier.SavingThrowModification(stat);
            return result;
        }

        public bool HasAdvantageAgainstMe(Creature actor, Creature victim)
        {
            foreach (var modifier in _allModifiers)
            {
                if (modifier.HasAdvantageAgainstMe(actor, victim))
                {
                    Console.WriteLine(
                        $"// Effect {modifier} causing advantage against {actor.Name} from {victim.Name}");
                    return true;
                }
            }

            return false;
        }

        public bool HasDisadvantageAgainstMe(Creature actor, Creature victim)
        {
            foreach (var modifier in _allModifiers)
            {
                if (modifier.HasDisadvantageAgainstMe(actor, victim))
                    return true;
            }

            return false;
        }

        public void DoAttack(Attack attackAction, Creature actor, Creature victim)
        {
            foreach (var modifier in _allModifiers)
            {
                modifier.DoAttack(attackAction, actor, victim);
            }
        }
    }
}