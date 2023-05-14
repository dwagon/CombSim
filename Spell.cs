using System;

namespace CombSim
{
    public class Spell : Action
    {
        protected int _reach;
        public int Level;
        protected DamageRoll _dmgRoll;

        public Spell(string name, int level, ActionCategory actionCategory) : base(name, actionCategory)
        {
            Level = level;
        }
        
        // Overwrite if the attack has a side effect
        protected virtual void SideEffect(Creature target)
        { }

        // Check to make sure we can cast this spell
        public override bool DoAction(Creature actor)
        {
            actor.DoCastSpell(this);
            return true;
        }
    }

    public class ToHitSpell : Spell
    {
        public ToHitSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }

        public override bool DoAction(Creature actor)
        {
            if (!actor.CanCastSpell(this)) return false;
            var enemy = actor.PickClosestEnemy();
            var oldLocation = actor.GetLocation();
            if (enemy == null) return false;
            while (actor.DistanceTo(enemy) > _reach)
                if (!actor.MoveTowards(enemy))
                    break;
            Console.WriteLine($"// {actor.Name} moved from {oldLocation} to {actor.GetLocation()}");

            if (actor.Game.DistanceTo(actor, enemy) <= _reach)
            {
                actor.DoCastSpell(this);
                DoAttack(actor, enemy);
                return true;
            }

            return false;
        }

        protected void DoAttack(Creature actor, Creature target, bool hasAdvantage = false,
            bool hasDisadvantage = false)
        {
            if (target.HasCondition(ConditionEnum.Paralyzed) && actor.DistanceTo(target) < 2) hasAdvantage = true;
            
            var roll = RollToHit(out var criticalHit, out var criticalMiss, hasAdvantage: hasAdvantage,
                hasDisadvantage: hasDisadvantage);
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name(), roll: roll, mods: actor.SpellAttackModifier());
            
            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
                ToHit = roll + actor.SpellAttackModifier(),
                DmgRoll = _dmgRoll,
                CriticalHit = criticalHit,
                CriticalMiss = criticalMiss,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
    
    /*
    public class DcSaveSpell: Spell {
        public DcSaveSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }
    }
    */
}