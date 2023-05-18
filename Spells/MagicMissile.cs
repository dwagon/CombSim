using System;

namespace CombSim.Spells
{
    public class MagicMissile : Spell
    {
        public MagicMissile() : base("Magic Missile", 1, ActionCategory.Action)
        {
            _reach = 120 / 5;
        }

        public override bool DoAction(Creature actor)
        {
            var numMissiles = 3;
            var hasCast = false;
            for (int i = 0; i < numMissiles; i++)
            {
                var enemy = actor.PickClosestEnemy();
                var oldLocation = actor.GetLocation();
                if (enemy == null) return false;
                while (actor.DistanceTo(enemy) > _reach)
                    if (!actor.MoveTowards(enemy))
                        break;
                Console.WriteLine($"// {actor.Name} moved from {oldLocation} to {actor.GetLocation()}");

                if (actor.Game.DistanceTo(actor, enemy) <= _reach)
                {
                    hasCast = true;
                    DoMissile(actor, enemy);
                }
            }
            if(hasCast)
                actor.DoCastSpell(this);
            return false;
        }

        private void DoMissile(Creature actor, Creature target)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());

            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
                ToHit = 999,
                DmgRoll = new DamageRoll("1d4+1", DamageTypeEnums.Force),
                CriticalHit = false,
                CriticalMiss = false,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}