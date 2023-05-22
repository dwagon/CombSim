using System;

namespace CombSim
{
    public class DashAction : Action
    {
        public DashAction() : base("Dash", ActionCategory.Action)
        {
        }

        public override int GetHeuristic(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (actor.DistanceTo(enemy) > actor.Speed) return 1;
            return 0;
        }

        public override void DoAction(Creature actor)
        {
            Console.WriteLine($"// Dashing");
            var enemy = actor.PickClosestEnemy();
            actor.MoveWithinReachOfEnemy(actor.Speed, enemy);
        }
    }
}