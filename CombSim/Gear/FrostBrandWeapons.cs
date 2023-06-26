namespace CombSim.Gear
{
    public class FrostBrandShortsword :
        MeleeWeapon
    {
        public FrostBrandShortsword() : base("Frost Brand Shortsword", new DamageRoll("1d6", DamageTypeEnums.Piercing))
        {
        }

        public override void Equip(Creature owner)
        {
            owner.AddResistance(DamageTypeEnums.Cold);
        }

        public override void SideEffect(Creature actor, Creature target)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name,
                attackName: "Frost Brand Effect");
            target.OnHitAttacked?.Invoke(this, new Creature.OnHitEventArgs
            {
                Source = actor,
                DmgRoll = new DamageRoll("1d6", DamageTypeEnums.Cold),
                AttackMessage = attackMessage,
                OnHitSideEffect = null
            });
        }

        public override int SideEffectHeuristic()
        {
            return 6;
        }
    }
}