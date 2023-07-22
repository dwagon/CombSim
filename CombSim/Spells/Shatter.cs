using System.Collections.Generic;
using System.Linq;

namespace CombSim.Spells
{
    public class Shatter : DcSaveSpell
    {
        private const int ShatterRadius = 20 / 5;

        public Shatter(int castAtSpellLevel) : base("Shatter", 2, ActionCategory.Action)
        {
            DmgRoll = new DamageRoll($"{3 + castAtSpellLevel - 2}d8", DamageTypeEnums.Thunder);
            SpellSavedEffect = SpellSavedEffect.DamageHalved;
            SpellSaveAgainst = StatEnum.Constitution;
            Reach = 60 / 5;
        }

        protected override void DoAction(Creature actor)
        {
            var bestTarget = PickBestTarget(actor);
            var damage = DmgRoll.Roll();
            foreach (var victim in bestTarget.GetCreaturesInCircle(ShatterRadius))
            {
                DoShatterAttack(actor, victim, damage);
            }
        }

        private void DoShatterAttack(Creature caster, Creature victim, Damage damage)
        {
            var attackMessage = new AttackMessage(attacker: caster.Name, victim: victim.Name, attackName: Name());
            victim.OnDcAttacked?.Invoke(this, new Creature.OnDcAttackedEventArgs
            {
                Source = caster,
                DcSaveStat = SpellSaveAgainst,
                DcSaveDc = caster.SpellSaveDc(),
                Damage = damage,
                DmgRoll = DmgRoll,
                SpellSavedEffect = SpellSavedEffect,
                AttackMessage = attackMessage,
                OnFailEffect = SideEffect
            });
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            if (!actor.CanCastSpell(this))
            {
                reason = "Spell not available";
                return 0;
            }

            var heuristic = new Heuristic(actor, this);
            var target = PickBestTarget(actor);
            if (target is null)
            {
                reason = "No enemy in range";
                return 0;
            }

            GetFriendsEnemies(actor, target, out var numFriends, out var numEnemies);
            heuristic.AddEnemies(numEnemies);
            heuristic.AddFriends(numFriends);
            heuristic.AddDamageRoll(DmgRoll);
            return heuristic.GetValue(out reason);
        }

        // Look at every enemy in range and see how many friends and enemies we would get if we
        // launched a shatter at them
        private Creature PickBestTarget(Creature caster)
        {
            var bestTargets = new List<(int, Creature)>();
            var potentialTargets = caster.GetCreaturesInCircle(Reach);
            foreach (var target in potentialTargets)
            {
                if (target.Team == caster.Team)
                    continue;
                GetFriendsEnemies(caster, target, out int numFriends, out int numEnemies);
                if (numFriends > 0)
                    continue;
                bestTargets.Add((numEnemies, target));
            }

            if (bestTargets.Count == 0)
                return null;

            bestTargets.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            return bestTargets.Last().Item2;
        }

        // Within the explosion radius centered on the {target}, how many friends and enemies are there
        private void GetFriendsEnemies(Creature caster, Creature target, out int numFriends, out int numEnemies)
        {
            numFriends = 0;
            numEnemies = 0;
            if (target == null) return;
            foreach (var critter in target.GetCreaturesInCircle(ShatterRadius))
            {
                if (caster.Team == critter.Team) numFriends++;
                else numEnemies++;
            }
        }
    }
}