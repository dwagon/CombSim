// All the various event args related to creatures

using System;

namespace CombSim
{
    public partial class Creature
    {
        public class OnTurnEndEventArgs : EventArgs
        {
            public Creature Creature;
        }

        public class OnTurnStartEventArgs : EventArgs
        {
            public Creature Creature;
        }

        public class OnAnyBeingKilledEventArgs : EventArgs
        {
            public Action Action;
            public Creature Source;
            public Creature Victim;
        }

        public class OnHitEventArgs : EventArgs
        {
            public Action Attack;
            public AttackMessage AttackMessage;
            public DamageRoll DmgRoll;
            public Action<Creature, Creature, Damage> OnHitSideEffect;
            public Creature Source;
        }

        public class OnToHitAttackedEventArgs : EventArgs
        {
            public Action Attack;
            public AttackMessage AttackMessage;
            public bool CriticalHit;
            public bool CriticalMiss;
            public DamageRoll DmgRoll;
            public Action<Creature, Creature, Damage> OnHitSideEffect;
            public Creature Source;
            public int ToHit;
        }

        public class OnDcAttackedEventArgs : EventArgs
        {
            public Action Attack;
            public AttackMessage AttackMessage;
            public Damage Damage;
            public int DcSaveDc;
            public StatEnum DcSaveStat;
            public DamageRoll DmgRoll;
            public Action<Creature, Creature, Damage> OnFailEffect;
            public Action<Creature, Creature, Damage> OnSucceedEffect;
            public Creature Source;
            public SpellSavedEffect SpellSavedEffect;
        }

        public class OnDealingDamageEventArgs : EventArgs
        {
            public Damage Damage;
            public Creature target;
        }

        public class OnTakingDamageEventArgs : EventArgs
        {
            public Damage Damage;
        }

        public class OnMovingEventArgs : EventArgs
        {
            public Location location;
            public Creature mover;
        }
    }
}