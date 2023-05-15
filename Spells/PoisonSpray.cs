namespace CombSim.Spells
{
    public class PoisonSpray: DcSaveSpell
    {
        public PoisonSpray() : base("Poison Spray", 0, ActionCategory.Action)
        {
            _dmgRoll = new DamageRoll("1d12", DamageTypeEnums.Poison);
            _dmgRollSaved = new DamageRoll("0d0", DamageTypeEnums.Poison);
            _reach = 5 / 5;
        }

        public override int GetHeuristic(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return 0;
            if (actor.DistanceTo(enemy) <= _reach)
                return 3;
            return 0;
        }
    }
}