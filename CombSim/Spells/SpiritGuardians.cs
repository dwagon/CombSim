using System;
using System.Collections.Generic;

namespace CombSim.Spells
{
    public class SpiritGuardians : Spell
    {
        private readonly SpiritGuardianEffect _effect = new SpiritGuardianEffect();

        public SpiritGuardians(int castAtSpellLevel) : base("Spirit Guardians", 3, ActionCategory.Action)
        {
            Concentration = true;
            DmgRoll = new DamageRoll($"{3 + castAtSpellLevel - 3}d8", DamageTypeEnums.Necrotic);
            TouchSpell = true; // Not really, but isn't ranged either for heuristic purposes
        }

        public override int GetHeuristic(Creature actor, out string reason)
        {
            if (actor.HasEffect("Spirit Guardians"))
            {
                reason = "Already active";
                return 0;
            }

            var heuristic = new Heuristic(actor, this);
            heuristic.AddEnemies(actor.Game.GetAllEnemies(actor).Count);
            heuristic.AddDamageRoll(DmgRoll);
            return heuristic.GetValue(out reason);
        }

        protected override void DoAction(Creature actor)
        {
            actor.AddEffect(_effect);
        }

        public override void EndConcentration(Creature actor)
        {
            actor.RemoveEffect(_effect);
        }
    }

    public class SpiritGuardianEffect : Effect
    {
        private const int MaxRange = 15 / 5;
        private readonly List<Creature> _attackedThisTurn;
        private readonly DamageRoll _dmgRoll = new DamageRoll("3d8", DamageTypeEnums.Radiant);
        private Creature _caster;

        public SpiritGuardianEffect() : base("Spirit Guardians")
        {
            _attackedThisTurn = new List<Creature>();
        }

        public override void Start(Creature target)
        {
            _caster = target;
            foreach (var creature in target.Game.GetAllEnemies(target))
            {
                creature.OnMoving += OnCreatureMoving;
                creature.OnTurnStart += OnCreatureStarting;
                if (creature.DistanceTo(target) <= MaxRange)
                {
                    SpiritGuardiansAttack(creature);
                }
            }
        }

        public override void End(Creature target)
        {
            Console.WriteLine($"// Spirit Guardians ended");
            _caster = target;
            foreach (var creature in target.Game.GetAllEnemies(target))
            {
                creature.OnMoving -= OnCreatureMoving;
                creature.OnTurnStart -= OnCreatureStarting;
            }
        }

        private void SpiritGuardiansAttack(Creature target)
        {
            if (_attackedThisTurn.Contains(target)) return;
            _attackedThisTurn.Add(target);
            var attackMessage = new AttackMessage(attacker: _caster.Name, victim: target.Name, attackName: Name);
            target.OnDcAttacked?.Invoke(this, new Creature.OnDcAttackedEventArgs
            {
                Source = _caster,
                DcSaveStat = StatEnum.Wisdom,
                DcSaveDc = target.SpellSaveDc(),
                DmgRoll = _dmgRoll,
                SpellSavedEffect = SpellSavedEffect.DamageHalved,
                AttackMessage = attackMessage,
                OnFailEffect = null
            });
        }

        // Creatures starting their turn in side the guardians
        private void OnCreatureStarting(object sender, Creature.OnTurnStartEventArgs e)
        {
            Creature target = e.Creature;
            _attackedThisTurn.Remove(target);
            if (target.DistanceTo(_caster) <= MaxRange && target.IsAlive())
            {
                Console.WriteLine($"// {target.Name} started turn in Spirit Guardians");
                SpiritGuardiansAttack(target);
            }
        }

        // Creatures entering the guardians this turn
        private void OnCreatureMoving(object sender, Creature.OnMovingEventArgs e)
        {
            Creature target = e.mover;
            if (target.DistanceTo(_caster) <= MaxRange && target.IsAlive())
            {
                Console.WriteLine($"// {target.Name} moved into Spirit Guardians");
                SpiritGuardiansAttack(target);
            }
        }
    }
}