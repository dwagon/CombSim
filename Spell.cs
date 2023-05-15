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

        protected virtual void DoAttack(Creature actor, Creature target, bool hasAdvantage = false,
            bool hasDisadvantage = false)
        {
            throw new NotImplementedException();
        }
    }

    public class ToHitSpell : Spell
    {
        public ToHitSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }

        protected override void DoAttack(Creature actor, Creature target, bool hasAdvantage = false,
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
    
    public class DcSaveSpell: Spell
    {
        protected DamageRoll _dmgRollSaved;
        
        public DcSaveSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }

        protected override void DoAttack(Creature actor, Creature target, bool hasAdvantage = false, bool hasDisadvantage = false)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());
            
            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
                DC = (StatEnum.Constitution, actor.SpellSaveDC()),
                DmgRoll = _dmgRoll,
                DmgRollSaved = _dmgRollSaved,
                CriticalHit = false,
                CriticalMiss = false,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}