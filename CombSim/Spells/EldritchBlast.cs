namespace CombSim.Spells
{
    public class EldritchBlast : ToHitSpell
    {
        private readonly int _casterLevel;

        public EldritchBlast(int casterLevel, int reach = 120 / 5, int dmgBonus = 0) : base("Eldritch Blast", 0,
            ActionCategory.Action)
        {
            Reach = reach;
            _casterLevel = casterLevel;
            DmgRoll = new DamageRoll("1d10", dmgBonus, DamageTypeEnums.Force);
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);

            heuristic.AddRepeat(GetNumRays());
            heuristic.AddDamageRoll(DmgRoll);
            return heuristic.GetValue(out reason);
        }

        private int GetNumRays()
        {
            if (_casterLevel >= 17) return 4;
            if (_casterLevel >= 11) return 3;
            if (_casterLevel >= 5) return 2;
            return 1;
        }

        protected override void DoAction(Creature actor)
        {
            var numRays = GetNumRays();
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            actor.MoveWithinReachOfCreature(1, enemy);
            if (actor.DistanceTo(actor.PickClosestEnemy()) > Reach) return;
            for (var ray = 0; ray < numRays; ray++)
                DoAttack(actor, enemy);
        }
    }
}