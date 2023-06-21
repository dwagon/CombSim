using System;

namespace CombSim
{
    public class DashAction : Action
    {
        public DashAction() : base("Dash", ActionCategory.Action)
        {
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var enemy = actor.PickClosestEnemy();
            if (actor.DistanceTo(enemy) > actor.Speed)
            {
                reason = $"Enemy {enemy.Name} needs to be closer";
                return 1;
            }

            reason = $"Enemy {enemy.Name} close enough";
            return 0;
        }

        protected override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            actor.Moves = Math.Max(actor.Moves, actor.Speed);
            actor.MoveWithinReachOfCreature(actor.Speed, enemy);
            actor.Moves = actor.Speed;
        }
    }
}