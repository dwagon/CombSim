namespace CombSim.Spells
{
    public class MagicMissile : Spell
    {
        private readonly int _castAtSpellLevel;
        private readonly DamageRoll _damageRoll = new DamageRoll("1d4+1", DamageTypeEnums.Force);

        public MagicMissile(int castAtSpellLevel) : base("Magic Missile", 1, ActionCategory.Action)
        {
            Reach = 120 / 5;
            _castAtSpellLevel = castAtSpellLevel;
        }

        private int NumberOfMissiles()
        {
            return 3 + _castAtSpellLevel - 1;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            heuristic.AddDamageRoll(_damageRoll);
            heuristic.AddRepeat(NumberOfMissiles());
            heuristic.NoRangeDisadvantage();
            return heuristic.GetValue(out reason);
        }

        protected override void DoAction(Creature actor)
        {
            var numMissiles = NumberOfMissiles();
            var hasCast = false;
            for (int i = 0; i < numMissiles; i++)
            {
                var enemy = actor.PickClosestEnemy();
                if (enemy == null) return;
                actor.MoveWithinReachOfCreature(Reach, enemy);

                if (actor.Game.DistanceTo(actor, enemy) <= Reach)
                {
                    hasCast = true;
                    DoMissile(actor, enemy);
                }
            }

            if (hasCast)
                actor.DoCastSpell(this);
        }

        private void DoMissile(Creature actor, Creature target)
        {
            DoHit(actor, target, _damageRoll);
        }
    }
}