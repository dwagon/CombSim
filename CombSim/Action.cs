namespace CombSim
{
    public enum ActionCategory
    {
        Action,
        Bonus,
        Reaction,
        Supplemental // Things like Action Surge which don't actually take up anything
    }

    public class Action
    {
        private readonly string _name;

        protected Action(string name, ActionCategory category)
        {
            _name = name;
            Category = category;
        }

        public ActionCategory Category { get; set; }

        public virtual int GetHeuristic(Creature actor, out string reason)
        {
            reason = "Default Action";
            return 0;
        }

        public string Name()
        {
            return _name;
        }

        public virtual void DoAction(Creature actor)
        {
        }

        protected int RollToHit(bool hasAdvantage = false,
            bool hasDisadvantage = false)
        {
            var roll = Dice.RollD20(hasAdvantage, hasDisadvantage, reason: "To Hit");
            return roll;
        }

        protected static bool IsCriticalHit(Creature actor, Creature target, int roll)
        {
            if (target.HasCondition(ConditionEnum.Paralyzed) && actor.DistanceTo(target) < 2)
            {
                return true;
            }

            return roll == 20 || roll >= actor.CriticalHitRoll;
        }

        protected static bool IsCriticalMiss(Creature actor, Creature target, int roll)
        {
            return roll == 1;
        }

        public bool HasAdvantage(Creature actor, Creature target)
        {
            bool hasAdvantage = false;
            bool hasDisadvantage = false;
            HasAdvantageDisadvantage(actor, target, ref hasAdvantage, ref hasDisadvantage);
            return hasAdvantage;
        }

        public bool HasDisadvantage(Creature actor, Creature target)
        {
            bool hasAdvantage = false;
            bool hasDisadvantage = false;
            HasAdvantageDisadvantage(actor, target, ref hasAdvantage, ref hasDisadvantage);
            return hasDisadvantage;
        }

        // Overwrite if the attack has a side effect
        protected virtual void SideEffect(Creature actor, Creature target)
        {
        }

        protected void DoHit(Creature actor, Creature target, DamageRoll damageRoll)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());

            target.OnHitAttacked?.Invoke(this, new Creature.OnHitEventArgs()
            {
                Source = actor,
                DmgRoll = damageRoll,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect,
                Attack = this
            });
        }

        protected void HasAdvantageDisadvantage(Creature actor, Creature target, ref bool hasAdvantage,
            ref bool hasDisadvantage)
        {
            if (target.HasAdvantageAgainstMe(actor))
            {
                hasAdvantage = true;
            }

            if (target.HasDisadvantageAgainstMe(actor))
            {
                hasDisadvantage = true;
            }

            if (actor.DistanceTo(target) < 2)
            {
                if (target.HasCondition(ConditionEnum.Paralyzed)) hasAdvantage = true;
                if (target.HasCondition(ConditionEnum.Prone)) hasAdvantage = true;
                if (actor.HasCondition(ConditionEnum.Prone)) hasDisadvantage = true;
            }
            else
            {
                if (target.HasCondition(ConditionEnum.Prone)) hasDisadvantage = true;
            }

            if (actor.HasCondition(ConditionEnum.Restrained)) hasDisadvantage = true;

            if (hasAdvantage && hasDisadvantage)
            {
                hasAdvantage = false;
                hasDisadvantage = false;
            }
        }
    }
}