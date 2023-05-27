using System;
using System.Collections.Generic;

namespace CombSim
{
    public class Effects
    {
        private readonly List<Effect> _effects = new List<Effect>();

        public override string ToString()
        {
            var names = new List<string>();
            foreach (var effect in _effects)
            {
                names.Add(effect.Name);
            }

            return string.Join(", ", names);
        }

        public void Add(Effect effect)
        {
            _effects.Add(effect);
        }

        public void Remove(Effect effect)
        {
            _effects.Remove(effect);
        }

        public bool HasAdvantageAgainstMe(Creature actor, Creature victim)
        {
            foreach (var effect in _effects)
            {
                if (effect.HasAdvantageAgainstMe(actor, victim))
                {
                    Console.WriteLine(
                        $"// Effect {effect.Name} causing advantage against {actor.Name} from {victim.Name}");
                    return true;
                }
            }

            return false;
        }

        public bool HasDisadvantageAgainstMe(Creature actor, Creature victim)
        {
            foreach (var effect in _effects)
            {
                if (effect.HasDisadvantageAgainstMe(actor, victim))
                    return true;
            }

            return false;
        }
    }
}