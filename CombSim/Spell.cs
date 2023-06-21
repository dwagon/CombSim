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
        protected bool Concentration = false;
        public bool TouchSpell = false;

        protected Spell(string name, int level, ActionCategory actionCategory) : base(name, actionCategory)
        {
            Level = level;
        }

        protected DamageRoll DmgRoll { get; set; }
        public int Reach { get; protected set; }

        public bool IsConcentration()
        {
            return Concentration;
        }

        public virtual void EndConcentration(Creature actor)
        {
        }

        protected override bool PreAction(Creature actor)
        {
            if (!actor.CanCastSpell(this)) return false;
            if (IsConcentration())
            {
                var oldSpell = actor.ConcentratingOn();
                if (oldSpell != null)
                {
                    oldSpell.EndConcentration(actor);
                }

                actor.ConcentrateOn(this);
            }

            return true;
        }

        protected override void PostAction(Creature actor)
        {
            actor.DoCastSpell(this);
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            heuristic.AddDamageRoll(DmgRoll);
            return heuristic.GetValue(out reason);
        }
    }
}