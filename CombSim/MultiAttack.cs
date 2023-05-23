using System.Collections.Generic;

namespace CombSim
{
    public class MultiAttack : Action
    {
        protected readonly List<Attack> Attacks;

        public MultiAttack(string name, ActionCategory actionCategory) : base(name, actionCategory)
        {
            Attacks = new List<Attack>();
        }

        public void AddAttack(Attack attack)
        {
            Attacks.Add(attack);
        }

        public override void DoAction(Creature actor)
        {
            foreach (var attack in Attacks)
            {
                attack.DoAction(actor);
            }
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            reason = "Undefined MultiAttack";
            var result = 0;
            foreach (var attack in Attacks)
            {
                result += attack.GetHeuristic(actor, out reason);
            }

            return result;
        }
    }
}