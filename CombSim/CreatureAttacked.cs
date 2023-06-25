using System;

namespace CombSim
{
    public partial class Creature
    {
        public EventHandler<OnDcAttackedEventArgs> OnDcAttacked;
        public EventHandler<OnDealingDamageEventArgs> OnDealingDamage;
        public EventHandler<OnHitEventArgs> OnHitAttacked;
        public EventHandler<OnTakingDamageEventArgs> OnTakingDamage;
        public EventHandler<OnToHitAttackedEventArgs> OnToHitAttacked;

        private void BeingAttackedInitialise()
        {
            OnToHitAttacked += AttackedByWeapon;
            OnDcAttacked += AttackedByDc;
            OnHitAttacked += BeingHit;
            OnDealingDamage += DamageDealt;
        }

        private void DamageDealt(object sender, OnDealingDamageEventArgs e)
        {
            DamageInflicted.Add(e.Damage);
        }

        // This creature has taken {damage} from an {action} by {source}
        private Damage TakeDamage(Damage damage, Creature source, Action action, out string dmgModifier)
        {
            damage = ModifyDamageForVulnerabilityOrResistance(damage, out dmgModifier);
            damage.hits = Math.Min(damage.hits, HitPoints);

            source.OnDealingDamage?.Invoke(this, new OnDealingDamageEventArgs
            {
                Damage = damage,
                target = this,
            });
            OnTakingDamage?.Invoke(this, new OnTakingDamageEventArgs { Damage = damage });

            HitPoints -= damage.hits;
            DamageReceived.Add(damage);
            if (HitPoints <= 0)
            {
                FallenUnconscious();
                Game.CreatureFallenUnconscious(source, this, action);
            }

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
            var damageNote = "";

            dmg = TakeDamage(dmg, e.Source, e.Attack, out string dmgModifier);
            damageNote += dmgModifier;
            e.AttackMessage.Result = $"Hit for ({e.DmgRoll}) {damageNote}:  {dmg}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
            if (e.OnHitSideEffect != null) e.OnHitSideEffect(e.Source, this, dmg);
        }

        private void AttackedByWeapon(object sender, OnToHitAttackedEventArgs e)
        {
            Damage dmg = null;

            if (e.CriticalMiss || e.ToHit <= ArmourClass((Attack)e.Attack))
            {
                e.AttackMessage.Result = "Miss";
                NarrationLog.LogMessage(e.AttackMessage.ToString());
                return;
            }

            if (e.DmgRoll != null)
            {
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

                dmg = TakeDamage(dmg, e.Source, e.Attack, out string dmgModifier);
                damageNote += dmgModifier;
                e.AttackMessage.Result = $"Hit for ({e.DmgRoll}) {damageNote}:  {dmg}";
            }

            NarrationLog.LogMessage(e.AttackMessage.ToString());
            if (e.OnHitSideEffect != null) e.OnHitSideEffect(e.Source, this, dmg);
        }

        // Attack that is a DC challenge
        private void AttackedByDc(object sender, OnDcAttackedEventArgs e)
        {
            var dmg = e.DmgRoll.Roll();
            var message = "";

            void Failed(Creature actor, Creature cause)
            {
                message = $"Failed save for {dmg}";
                if (e.OnFailEffect != null) e.OnFailEffect(e.Source, this, dmg);
            }

            void Saved(Creature actor, Creature cause)
            {
                switch (e.SpellSavedEffect)
                {
                    case SpellSavedEffect.DamageHalved:
                        dmg /= 2;
                        message = "Saved for half damage";
                        break;
                    case SpellSavedEffect.NoDamage:
                        message = "Saved for no damage";
                        dmg = new Damage(0, DamageTypeEnums.None);
                        break;
                }

                if (e.OnSucceedEffect != null) e.OnSucceedEffect(e.Source, this, dmg);
            }

            var dcChallenge = new DcChallenge(e.DcSaveStat, e.DcSaveDc, Saved, Failed);
            dcChallenge.MakeSave(this, e.Source, out int roll);

            dmg = TakeDamage(dmg, e.Source, e.Attack, out string dmgModifier);
            e.AttackMessage.Result =
                $"{message} :{roll} vs {e.DcSaveStat} DC {e.DcSaveDc} ({e.DmgRoll}) {dmgModifier}: {dmg}";
            NarrationLog.LogMessage(e.AttackMessage.ToString());
        }
    }
}