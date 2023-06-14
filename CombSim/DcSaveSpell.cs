namespace CombSim
{
    public class DcSaveSpell : AttackSpell
    {
        protected StatEnum SpellSaveAgainst;
        protected SpellSavedEffect SpellSavedEffect;

        protected DcSaveSpell(string name, int level, ActionCategory actionCategory) : base(name, level, actionCategory)
        {
        }

        protected override void DoAction(Creature actor)
        {
            var target = actor.PickClosestEnemy();
            actor.MoveWithinReachOfCreature(Reach, target);
            if (actor.Game.DistanceTo(actor, target) > Reach) return;
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