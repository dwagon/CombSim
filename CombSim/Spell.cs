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
        protected int Reach;
        public readonly int Level;
        protected DamageRoll DmgRoll;

        protected Spell(string name, int level, ActionCategory actionCategory) : base(name, actionCategory)
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
            actor.MoveWithinReachOfEnemy(Reach, enemy);
            if (actor.Game.DistanceTo(actor, enemy) <= Reach)
            {
                actor.DoCastSpell(this);
                DoAttack(actor, enemy);
                return true;
            }

            return false;
        }
        
        public override int GetHeuristic(Creature actor)
        {
            if (!actor.CanCastSpell(this)) return 0;
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return 0;
            if (actor.DistanceTo(enemy) <= Reach)
                return 2 + 2*Level;
            return 0;
        }

        protected virtual void DoAttack(Creature actor, Creature target, bool hasAdvantage = false,
            bool hasDisadvantage = false)
        {
            throw new NotImplementedException();
        }
    }

    public class ToHitSpell : Spell
    {
        protected ToHitSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }

        protected override void DoAttack(Creature actor, Creature target, bool hasAdvantage = false,
            bool hasDisadvantage = false)
        {
            if (target.HasCondition(ConditionEnum.Paralyzed) && actor.DistanceTo(target) < 2) hasAdvantage = true;
            
            var roll = RollToHit(hasAdvantage: hasAdvantage,
                hasDisadvantage: hasDisadvantage);
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name(), roll: roll, mods: actor.SpellAttackModifier());
            
            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
                ToHit = roll + actor.SpellAttackModifier(),
                DmgRoll = DmgRoll,
                CriticalHit = IsCriticalHit(actor, target, roll),
                CriticalMiss = IsCriticalMiss(actor, target, roll),
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
    
    public class DcSaveSpell: Spell
    {
        protected SpellSavedEffect SpellSavedEffect;
        
        protected DcSaveSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }

        protected override void DoAttack(Creature actor, Creature target, bool hasAdvantage = false, bool hasDisadvantage = false)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());
            
            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
                Dc = (StatEnum.Constitution, actor.SpellSaveDc()),
                DmgRoll = DmgRoll,
                SpellSavedEffect = SpellSavedEffect,
                CriticalHit = false,
                CriticalMiss = false,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}