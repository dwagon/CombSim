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

        // Overwrite if the attack has a side effect
        protected virtual void SideEffect(Creature actor, Creature target)
        {
        }

        public override void DoAction(Creature actor)
        {
            if (!actor.CanCastSpell(this)) return;

            var enemy = actor.PickClosestEnemy();
            actor.MoveWithinReachOfEnemy(Reach, enemy);
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

    public class ToHitSpell : Spell
    {
        protected ToHitSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }

        protected override void DoAttack(Creature actor, Creature target)
        {
            var hasAdvantage = false;
            var hasDisadvantage = false;

            HasAdvantageDisadvantage(actor, target, ref hasAdvantage, ref hasDisadvantage);

            var roll = RollToHit(hasAdvantage: hasAdvantage, hasDisadvantage: hasDisadvantage);
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name(),
                roll: roll, mods: actor.SpellAttackModifier());

            target.OnToHitAttacked?.Invoke(this, new Creature.OnToHitAttackedEventArgs()
            {
                Source = actor,
                ToHit = roll + actor.SpellAttackModifier(),
                DmgRoll = DmgRoll,
                CriticalHit = IsCriticalHit(actor, target, roll),
                CriticalMiss = IsCriticalMiss(actor, target, roll),
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }

    public class DcSaveSpell : Spell
    {
        protected StatEnum SpellSaveAgainst;
        protected SpellSavedEffect SpellSavedEffect;

        protected DcSaveSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }

        protected override void DoAttack(Creature actor, Creature target)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());

            target.OnDcAttacked?.Invoke(this, new Creature.OnDcAttackedEventArgs()
            {
                Source = actor,
                DcSaveStat = SpellSaveAgainst,
                DcSaveDc = actor.SpellSaveDc(),
                DmgRoll = DmgRoll,
                SpellSavedEffect = SpellSavedEffect,
                AttackMessage = attackMessage,
                OnFailEffect = SideEffect
            });
        }
    }
}