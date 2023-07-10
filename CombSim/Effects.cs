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

        public bool HasEffect(string name)
        {
            if (GetEffectByName(name) == null) return false;
            return true;
        }

        public Effect GetEffectByName(string name)
        {
            foreach (var effect in _effects)
            {
                if (effect.Name == name)
                {
                    return effect;
                }
            }

            return null;
        }
    }
}