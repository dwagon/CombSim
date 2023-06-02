using System;

public enum SpellSavedEffect
{
    NoDamage,
    DamageHalved
}

namespace CombSim
{
    public class Spell : Action
    {
        public readonly int Level;
        public bool TouchSpell = false;

        protected Spell(string name, int level, ActionCategory actionCategory) : base(name, actionCategory)
        {
            Level = level;
        }

        protected DamageRoll DmgRoll { get; set; }
        public int Reach { get; protected set; }

        public override void DoAction(Creature actor)
        {
            if (!actor.CanCastSpell(this)) return;

            var enemy = actor.PickClosestEnemy();
            actor.MoveWithinReachOfCreature(Reach, enemy);
            if (actor.Game.DistanceTo(actor, enemy) <= Reach)
            {
                actor.DoCastSpell(this);
                DoAttack(actor, enemy);
            }
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            heuristic.AddDamageRoll(DmgRoll);
            return heuristic.GetValue(out reason);
        }

        protected virtual void DoAttack(Creature actor, Creature target)
        {
            throw new NotImplementedException();
        }
    }
}