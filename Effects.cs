using System;
using System.Collections.Generic;

namespace CombSim
{
    public class Effects
    {
        private readonly List<Effect> _effects = new List<Effect>();        

        public override string ToString()
        {
            return string.Join(", ", _effects);
        }

        public void Add(Effect effect)
        {
            _effects.Add(effect);
        }

        public void Remove(Effect effect)
        {
            _effects.Remove(effect);
        }
    }
}