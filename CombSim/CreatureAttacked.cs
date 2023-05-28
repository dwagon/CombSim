using System;

namespace CombSim
{
    public partial class Creature
    {
        public EventHandler<OnDcAttackedEventArgs> OnDcAttacked;
        public EventHandler<OnHitEventArgs> OnHitAttacked;
        public EventHandler<OnToHitAttackedEventArgs> OnToHitAttacked;

        private void BeingAttackedInitialise()
        {
            OnToHitAttacked += AttackedByWeapon;
            OnDcAttacked += AttackedbyDc;
            OnHitAttacked += BeingHit;
        }

        private Damage TakeDamage(Damage damage, out string dmgModifier)
        {
            damage = ModifyDamageForVulnerabilityOrResistance(damage, out dmgModifier);
            HitPoints -= damage.hits;
            DamageReceived.Add(damage);
            if (HitPoints <= 0) FallenUnconscious();
            return damage;
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

        // You just took damage
        private void BeingHit(object sender, OnHitEventArgs e)
        {
            var dmg = e.DmgRoll.Roll();
            ;
            var damageNote = "";

            dmg = TakeDamage(dmg, out string dmgModifier);
            damageNote += dmgModifier;
            e.AttackMessage.Result = $"Hit for ({e.DmgRoll}) {damageNote}:  {dmg}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
            if (e.OnHitSideEffect != null) e.OnHitSideEffect(e.Source, this);
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

            dmg = TakeDamage(dmg, out string dmgModifier);
            damageNote += dmgModifier;
            e.AttackMessage.Result = $"Hit for ({e.DmgRoll}) {damageNote}:  {dmg}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
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
                        message = $"Saved for half damage";
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

            dmg = TakeDamage(dmg, out string dmgModifier);
            e.AttackMessage.Result =
                $"{message} :{roll} vs {e.DcSaveStat} DC {e.DcSaveDc} ({e.DmgRoll}) {dmgModifier}: {dmg}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
        }

        public class OnHitEventArgs : EventArgs
        {
            public AttackMessage AttackMessage;
            public DamageRoll DmgRoll;
            public Action<Creature, Creature> OnHitSideEffect;
            public Creature Source;
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