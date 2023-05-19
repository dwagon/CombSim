using System.Collections.Generic;

namespace CombSim.Spells
{
    public class Thunderclap: DcSaveSpell
    {
        public Thunderclap() : base("Thunderclap", 0, ActionCategory.Action)
        {
            Reach = 5 / 5;
            DmgRoll = new DamageRoll("1d6", DamageTypeEnums.Thunder);
            SpellSavedEffect = SpellSavedEffect.NoDamage;
        }

        public override int GetHeuristic(Creature actor)
        {
            var numEnemies = 0;
            var numFriends = 0;
            foreach (var creature in actor.GetNeighbourCreatures())
            {
                if (creature.Team == actor.Team) numFriends++;
                else numEnemies++;
            }

            if (numFriends > 0) return 0;
            return 2 + numEnemies * 2;
        }

        public override bool DoAction(Creature actor)
        {
            if (!actor.CanCastSpell(this)) return false;
            actor.DoCastSpell(this);
            foreach (var target in actor.GetNeighbourCreatures())
            {
                DoThunderclapAttack(actor, target);
            }
            return true;
        }

        private void DoThunderclapAttack(Creature actor, Creature target)
        {
            var attackMessage = new AttackMessage(attacker: actor.Name, victim: target.Name, attackName: Name());

            target.OnAttacked?.Invoke(this, new Creature.OnAttackedEventArgs
            {
                Source = actor,
                Action = this,
                Dc = (SpellSaveAgainst, actor.SpellSaveDc()),
                DmgRoll = DmgRoll,
                SpellSavedEffect = SpellSavedEffect,
                CriticalHit = false,
                CriticalMiss = false,
                AttackMessage = attackMessage,
                OnHitSideEffect = SideEffect
            });
        }
    }
}