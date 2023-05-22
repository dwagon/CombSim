using System;
using System.Linq;

namespace CombSim.Spells
{
    public class BurningHands : DcSaveSpell
    {
        public BurningHands() : base("Burning Hands", 1, ActionCategory.Action)
        {
            DmgRoll = new DamageRoll("3d6", DamageTypeEnums.Fire);
            SpellSavedEffect = SpellSavedEffect.DamageHalved;
            SpellSaveAgainst = StatEnum.Dexterity;
            Reach = 15 / 5;
        }

        public override int GetHeuristic(Creature actor)
        {
            if (!actor.CanCastSpell(this)) return 0;
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return 0;
            GetBestDirection(actor, out var numEnemiesCovered);
            return numEnemiesCovered * 5;
        }

        private GridDirection GetBestDirection(Creature actor, out int numEnemiesCovered)
        {
            var bestDirection = new GridDirection();
            var bestScore = 0;

            foreach (var direction in Enum.GetValues(typeof(GridDirection)).Cast<GridDirection>())
            {
                FriendEnemyCount(actor, Reach, direction, out var numFriends, out var numEnemies);

                if (numFriends != 0) continue;
                if (bestScore > numEnemies) continue;
                bestScore = numEnemies;
                bestDirection = direction;
            }

            numEnemiesCovered = bestScore;
            Console.WriteLine($"//\t\tBest Direction = {bestDirection} with {numEnemiesCovered} enemies");

            return bestDirection;
        }

        private static void FriendEnemyCount(Creature actor, int range, GridDirection direction, out int numFriends,
            out int numEnemies)
        {
            numEnemies = 0;
            numFriends = 0;
            foreach (var location in actor.GetConeLocations(range, direction))
            {
                var creature = actor.Game.GetCreatureAtLocation(location);
                if (creature is null) continue;
                if (creature.Team == actor.Team) numFriends++;
                else numEnemies++;
            }
        }

        public override void DoAction(Creature actor)
        {
            if (!actor.CanCastSpell(this)) return;
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            actor.MoveWithinReachOfEnemy(1, enemy);
            if (actor.DistanceTo(actor.PickClosestEnemy()) > Reach) return;
            actor.DoCastSpell(this);
            DoBurningHandsAttack(actor);
        }

        private void DoBurningHandsAttack(Creature actor)
        {
            var bestDirection = GetBestDirection(actor, out _);
            foreach (var location in actor.GetConeLocations(Reach, bestDirection))
            {
                if (location == actor.GetLocation()) continue;
                var target = actor.Game.GetCreatureAtLocation(location);
                if (target is null) continue;
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
}