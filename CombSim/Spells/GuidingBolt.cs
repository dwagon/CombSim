using System;

namespace CombSim.Spells
{
    public class GuidingBolt : ToHitSpell
    {
        private readonly GuidingBoltEffect _effect = new GuidingBoltEffect();
        private Creature _target;
        private int _turnsToGo;

        public GuidingBolt(int castAtSpellLevel) : base("Guiding Bolt", 1, ActionCategory.Action)
        {
            Reach = 120 / 5;
            DmgRoll = new DamageRoll($"{4 + castAtSpellLevel - 1}d6", DamageTypeEnums.Radiant);
        }

        protected override void SideEffect(Creature actor, Creature target, Damage damage)
        {
            _turnsToGo = 2; // End next turn
            Console.WriteLine($"// Guiding Bolt Side Effect Actor={actor.Name} Target={target.Name}");
            _target = target;
            actor.OnTurnEnd += TurnEnd;
            _target.AddEffect(_effect);
        }

        private void TurnEnd(object sender, Creature.OnTurnEndEventArgs e)
        {
            if (--_turnsToGo > 0)
                return;

            Console.WriteLine("// Removing Guiding Bolt effect");
            var creature = (Creature)sender;
            _target.RemoveEffect(_effect);
            creature.OnTurnEnd -= TurnEnd;
        }

        // The next attack roll made against this target before the end of your next turn has advantage,
        // thanks to the mystical dim light glittering on the target until then.
        private class GuidingBoltEffect : Effect
        {
            public GuidingBoltEffect() : base("Guiding Bolt Effect")
            {
            }

            public override bool HasAdvantageAgainstMe(Creature target, Creature attacker)
            {
                Console.WriteLine($"// Advantage from Guiding Bolt for attack on {target.Name} from {attacker.Name}");
                target.RemoveEffect(this);
                return true;
            }

            public override void End(Creature target)
            {
                Console.WriteLine($"// Guiding Bolt Ended");
            }
        }
    }
}