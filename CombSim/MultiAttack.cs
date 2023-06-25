using System.Collections.Generic;

namespace CombSim
{
    public class MultiAttack : Action
    {
        private readonly List<Attack> _attacks;

        public MultiAttack(string name, ActionCategory actionCategory) : base(name, actionCategory)
        {
            _attacks = new List<Attack>();
        }

        public void AddAttack(Attack attack)
        {
            _attacks.Add(attack);
        }

        public void AddAttack(Weapon weapon)
        {
            foreach (var attack in weapon.GetActions())
            {
                _attacks.Add((Attack)attack);
            }
        }

        public override void PerformAction(Creature actor)
        {
            foreach (var attack in _attacks)
            {
                attack.PerformAction(actor);
            }
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            reason = "Undefined MultiAttack";
            var result = 0;
            foreach (var attack in _attacks)
            {
                result += attack.GetHeuristic(actor, out reason);
            }

            return result;
        }
    }
}