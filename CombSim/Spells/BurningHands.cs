using System;
using System.Linq;

namespace CombSim.Spells
{
    public class BurningHands : DcSaveSpell
    {
        private const int ArcSize = 15 / 5;

        public BurningHands(int castAtSpellLevel) : base("Burning Hands", 1, ActionCategory.Action)
        {
            var dmgString = $"{3 + castAtSpellLevel - 1}d6";
            DmgRoll = new DamageRoll(dmgString, DamageTypeEnums.Fire);
            SpellSavedEffect = SpellSavedEffect.DamageHalved;
            SpellSaveAgainst = StatEnum.Dexterity;
            Reach = 5 / 5;
            TouchSpell = true;
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            var heuristic = new Heuristic(actor, this);

            GetBestDirection(actor, out var numEnemiesCovered);
            heuristic.AddEnemies(numEnemiesCovered);
            heuristic.AddDamageRoll(DmgRoll);
            return heuristic.GetValue(out reason);
        }

        private GridDirection GetBestDirection(Creature actor, out int numEnemiesCovered)
        {
            var bestDirection = new GridDirection();
            var bestScore = 0;

            foreach (var direction in Enum.GetValues(typeof(GridDirection)).Cast<GridDirection>())
            {
                FriendEnemyCount(actor, ArcSize, direction, out var numFriends, out var numEnemies);

                if (numFriends != 0) continue;
                if (bestScore > numEnemies) continue;
                bestScore = numEnemies;
                bestDirection = direction;
            }

            numEnemiesCovered = bestScore;

            return bestDirection;
        }

        private static void FriendEnemyCount(Creature actor, int range, GridDirection direction, out int numFriends,
            out int numEnemies)
        {
            numEnemies = 0;
            numFriends = 0;
            foreach (var creature in actor.GetCreaturesInCone(range, direction))
            {
                if (creature.Team == actor.Team) numFriends++;
                else numEnemies++;
            }
        }

        protected override void DoAction(Creature actor)
        {
            var enemy = actor.PickClosestEnemy();
            if (enemy == null) return;
            actor.MoveWithinReachOfCreature(1, enemy);
            if (actor.DistanceTo(actor.PickClosestEnemy()) > Reach) return;
            DoBurningHandsAttack(actor);
        }

        private void DoBurningHandsAttack(Creature actor)
        {
            var bestDirection = GetBestDirection(actor, out _);
            foreach (var target in actor.GetCreaturesInCone(ArcSize, bestDirection))
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
}