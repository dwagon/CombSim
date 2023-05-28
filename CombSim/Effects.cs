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
                if (!effect.Hidden)
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

        public bool HasEffect(string name)
        {
            foreach (var effect in _effects)
            {
                if (effect.Name == name) return true;
            }

            return false;
        }

        public void DoAttack(Attack attackAction, Creature actor, Creature victim)
        {
            if (!victim.IsAlive()) return;
            Console.WriteLine($"// Effects.DoAttack()");
            foreach (var effect in _effects)
            {
                effect.DoAttack(attackAction, actor, victim);
            }
        }
    }
}