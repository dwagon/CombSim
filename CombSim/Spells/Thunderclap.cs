namespace CombSim.Spells
{
    public class Thunderclap : DcSaveSpell
    {
        public Thunderclap(int casterLevel) : base("Thunderclap", 0, ActionCategory.Action)
        {
            Reach = 5 / 5;
            if (casterLevel >= 11) DmgRoll = new DamageRoll("3d6", DamageTypeEnums.Thunder);
            else if (casterLevel >= 5) DmgRoll = new DamageRoll("2d6", DamageTypeEnums.Thunder);
            else DmgRoll = new DamageRoll("1d6", DamageTypeEnums.Thunder);
            SpellSavedEffect = SpellSavedEffect.NoDamage;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);
            var numEnemies = 0;
            var numFriends = 0;
            foreach (var creature in actor.GetNeighbourCreatures())
            {
                if (creature.Team == actor.Team) numFriends++;
                else numEnemies++;
            }

            heuristic.AddFriends(numFriends);
            heuristic.AddEnemies(numEnemies);
            heuristic.AddDamageRoll(DmgRoll);
            return heuristic.GetValue(out reason);
        }

        protected override void DoAction(Creature actor)
        {
            foreach (var target in actor.GetNeighbourCreatures())
            {
                DoThunderclapAttack(actor, target);
            }
        }

        private void DoThunderclapAttack(Creature actor, Creature target)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());

            target.OnDcAttacked?.Invoke(this, new Creature.OnDcAttackedEventArgs()
            {
                Source = actor,
                DcSaveStat = SpellSaveAgainst,
                DcSaveDc = actor.SpellSaveDc(),
                DmgRoll = DmgRoll,
                SpellSavedEffect = SpellSavedEffect,
                AttackMessage = attackMessage,
                OnFailEffect = SideEffect
            });
        }
    }
}