using System;

namespace CombSim
{
    public class Potion : Equipment
    {
        private bool _consumed;

        protected Potion(string name) : base(name)
        {
            AddAction(new DrinkPotion($"Drink {name}", this));
            _consumed = false;
        }

        protected virtual void PotionEffect(Creature drinker)
        {
            throw new NotImplementedException();
        }

        private bool IsConsumed()
        {
            return _consumed;
        }

        private void Consume()
        {
            _consumed = true;
        }

        protected virtual int GetHeuristic(Creature drinker, out string reason)
        {
            reason = "Undefined potion";
            return 0;
        }

        private class DrinkPotion : Action
        {
            private readonly Potion _potion;

            public DrinkPotion(string name, Potion potion) : base(name, ActionCategory.Bonus)
            {
                _potion = potion;
            }

            public override int GetHeuristic(Creature actor, out string reason)
            {
                if (_potion.IsConsumed())
                {
                    reason = "Potion already consumed";
                    return 0;
                }

                return _potion.GetHeuristic(actor, out reason);
            }

            public override void DoAction(Creature actor)
            {
                if (_potion.IsConsumed()) return;
                _potion.PotionEffect(actor);
                _potion.Consume();
            }
        }
    }
}