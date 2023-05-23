using System;

namespace CombSim
{
    public partial class Creature
    {
        public EventHandler<OnDcAttackedEventArgs> OnDcAttacked;
        public EventHandler<OnToHitAttackedEventArgs> OnToHitAttacked;

        private void BeingAttackedInitialise()
        {
            OnToHitAttacked += AttackedByWeapon;
            OnDcAttacked += AttackedbyDc;
        }

        private void TakeDamage(Damage damage)
        {
            HitPoints -= damage.hits;
            DamageReceived.Add(damage);
            if (HitPoints <= 0) FallenUnconscious();
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

        private void AttackedByWeapon(object sender, OnToHitAttackedEventArgs e)
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
        private void AttackedbyDc(object sender, OnDcAttackedEventArgs e)
        {
            var dmg = e.DmgRoll.Roll();
            string message = "";

            void Failed(Creature actor, Creature cause)
            {
                message = $"Failed save for {dmg}";
                if (e.OnFailEffect != null) e.OnFailEffect(e.Source, this);
            }

            void Saved(Creature actor, Creature cause)
            {
                switch (e.SpellSavedEffect)
                {
                    case SpellSavedEffect.DamageHalved:
                        dmg /= 2;
                        message = $"Saved for {dmg}";
                        break;
                    case SpellSavedEffect.NoDamage:
                        message = $"Saved for no damage";
                        dmg = new Damage(0, DamageTypeEnums.None);
                        break;
                }

                if (e.OnSucceedEffect != null) e.OnSucceedEffect(e.Source, this);
            }

            var dcChallenge = new DcChallenge(e.DcSaveStat, e.DcSaveDc, Saved, Failed);
            dcChallenge.MakeSave(this, e.Source, out int roll);

            dmg = ModifyDamageForVulnerabilityOrResistance(dmg, out string dmgModifier);
            e.AttackMessage.Result = $"{message} :{roll} vs {e.DcSaveStat} DC {e.DcSaveDc} ({e.DmgRoll}) {dmgModifier}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
            TakeDamage(dmg);
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

        public class OnDcAttackedEventArgs : EventArgs
        {
            public AttackMessage AttackMessage;
            public int DcSaveDc;
            public StatEnum DcSaveStat;
            public DamageRoll DmgRoll;
            public Action<Creature, Creature> OnFailEffect;
            public Action<Creature, Creature> OnSucceedEffect;
            public Creature Source;
            public SpellSavedEffect SpellSavedEffect;
        }
    }
}