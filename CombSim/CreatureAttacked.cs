using System;

namespace CombSim
{
    public partial class Creature
    {
        public EventHandler<OnSpellDcAttackedEventArgs> OnSpellDcAttacked;
        public EventHandler<OnToHitAttackedEventArgs> OnToHitAttacked;

        private void BeingAttackedInitialise()
        {
            OnToHitAttacked += WeaponAttacked;
            OnSpellDcAttacked += SpellDcAttack;
        }

        private void TakeDamage(Damage damage)
        {
            HitPoints -= damage.hits;
            DamageReceived.Add(damage);
            if (HitPoints <= 0) FallenUnconscious();
        }

        public void MoveWithinReachOfEnemy(int reach, Creature enemy)
        {
            var oldLocation = GetLocation();
            if (enemy == null) return;
            while (DistanceTo(enemy) > reach)
                if (!MoveTowards(enemy))
                    break;
            if (oldLocation != GetLocation())
            {
                Console.WriteLine($"// {Name} moved from {oldLocation} to {GetLocation()}");
            }
        }

        public Creature PickClosestEnemy()
        {
            return Game.PickClosestEnemy(this);
        }

        private Damage ModifyDamageForVulnerabilityOrResistance(Damage dmg, out string dmgModifier)
        {
            dmgModifier = "";
            if (Vulnerable.Contains(dmg.type))
            {
                dmgModifier = " (Vulnerable) ";
                dmg *= 2;
            }
            else if (Resistant.Contains(dmg.type))
            {
                dmgModifier = " (Resistant) ";
                dmg /= 2;
            }
            else if (Immune.Contains(dmg.type))
            {
                dmg = new Damage(0, dmg.type);
                dmgModifier = " (Immune) ";
            }

            return dmg;
        }

        private void WeaponAttacked(object sender, OnToHitAttackedEventArgs e)
        {
            if (e.CriticalMiss || e.ToHit <= ArmourClass)
            {
                e.AttackMessage.Result = "Miss";
                NarrationLog.LogMessage(e.AttackMessage.ToString());
                return;
            }

            Damage dmg;
            var damageNote = "";
            if (e.CriticalHit)
            {
                damageNote += " (Critical Hit) ";
                dmg = e.DmgRoll.Roll(max: true) + e.DmgRoll.Roll();
            }
            else
            {
                dmg = e.DmgRoll.Roll();
            }

            dmg = ModifyDamageForVulnerabilityOrResistance(dmg, out string dmgModifier);
            damageNote += dmgModifier;
            e.AttackMessage.Result = $"Hit for {dmg} damage ({e.DmgRoll}) {damageNote}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
            TakeDamage(dmg);
            if (e.OnHitSideEffect != null) e.OnHitSideEffect(e.Source, this);
        }

        // Attack that is a DC challenge
        private void SpellDcAttack(object sender, OnSpellDcAttackedEventArgs e)
        {
            var dmg = e.DmgRoll.Roll();

            if (MakeSavingThrow(e.DcSaveStat, e.DcSaveDc))
            {
                switch (e.SpellSavedEffect)
                {
                    case SpellSavedEffect.DamageHalved:
                        dmg /= 2;
                        e.AttackMessage.Result = $"Saved for {dmg}";
                        break;
                    case SpellSavedEffect.NoDamage:
                        e.AttackMessage.Result = $"Saved for no damage";
                        dmg = new Damage(0, DamageTypeEnums.None);
                        break;
                }
            }
            else
            {
                e.AttackMessage.Result = $"Failed save for {dmg}";
            }

            dmg = ModifyDamageForVulnerabilityOrResistance(dmg, out string dmgModifier);
            e.AttackMessage.Result = $"Hit for {dmg} damage ({e.DmgRoll}) {dmgModifier}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
            TakeDamage(dmg);
            if (e.OnHitSideEffect != null) e.OnHitSideEffect(e.Source, this);
        }

        public class OnToHitAttackedEventArgs : EventArgs
        {
            public AttackMessage AttackMessage;
            public bool CriticalHit;
            public bool CriticalMiss;
            public DamageRoll DmgRoll;
            public Action<Creature, Creature> OnHitSideEffect;
            public Creature Source;
            public int ToHit;
        }

        public class OnSpellDcAttackedEventArgs : EventArgs
        {
            public AttackMessage AttackMessage;
            public int DcSaveDc;
            public StatEnum DcSaveStat;
            public DamageRoll DmgRoll;
            public Action<Creature, Creature> OnHitSideEffect;
            public Creature Source;
            public SpellSavedEffect SpellSavedEffect;
        }
    }
}