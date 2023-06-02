// General calculation of the effect of an attack
// Based on the hit points damage it will do to the enemy team

using System;

namespace CombSim
{
    public class Heuristic
    {
        private Creature _actor;
        private AttackType _attackType;
        private DcAttack _dcAttack;
        private int _enemies = 1;
        private int _friends = 0;
        private bool _hasAdvantage = false;
        private bool _hasDisadvantage = false;

        private int _maxDamage;
        private MeleeAttack _meleeAttack;
        private RangedAttack _rangedAttack;
        private bool _rangeDisadvantage = true;
        private int _repeats = 1;
        private Spell _spell;

        public Heuristic(Creature actor, MeleeAttack attack)
        {
            // Modify by chance to hit
            _actor = actor;
            _meleeAttack = attack;
            _attackType = AttackType.MeleeAttack;
            AddDamageRoll(attack.DamageRoll);
        }

        public Heuristic(Creature actor, DcAttack attack)
        {
            // TODO: Modify by chance to save && impact of save
            _actor = actor;
            _dcAttack = attack;
            _attackType = AttackType.DcAttack;
            AddDamageRoll(attack.DamageRoll);
        }

        public Heuristic(Creature actor, RangedAttack attack)
        {
            // TODO: Modify by chance to hit
            _actor = actor;
            _rangedAttack = attack;
            _attackType = AttackType.RangedAttack;
            AddDamageRoll(attack.DamageRoll);
        }

        public Heuristic(Creature actor)
        {
            _actor = actor;
        }

        public Heuristic(Creature actor, Spell spell)
        {
            _actor = actor;
            _spell = spell;
            _attackType = spell.TouchSpell ? AttackType.TouchSpellAttack : AttackType.RangedSpellAttack;
        }

        public int GetValue(out string reason)
        {
            int value;
            reason = "H: undefined";
            if (_attackType == AttackType.RangedSpellAttack || _attackType == AttackType.TouchSpellAttack)
            {
                if (!_actor.CanCastSpell(_spell))
                {
                    reason = $"H: {_actor.Name} can't cast {_spell.Name()}";
                    return 0;
                }
            }

            if (_friends > 0)
            {
                reason = $"H: Would affect {_friends} allies";
                return 0;
            }

            var rangeValue = RangeConsiderations(out string addendum);
            if (!rangeValue)
            {
                reason = $"H: {addendum}";
                return 0;
            }

            if (_maxDamage == 0)
            {
                throw new Exception($"No damage specified: {_attackType}");
            }

            // addendum += $"[_maxDamage {_maxDamage}; _repeats {_repeats}; _enemy {_enemies}]";
            value = _maxDamage * _repeats;
            value *= _enemies;

            reason = $"H: Max Damage {value} {addendum}";
            if (_hasDisadvantage)
            {
                reason += " (Disadvantage)";
                value = (int)(value * 0.75f);
            }

            return value;
        }

        private bool RangeConsiderations(out string addendum)
        {
            var enemy = _actor.PickClosestEnemy();
            var distance = _actor.DistanceTo(enemy);

            addendum = $"[distance: {distance}] ";

            switch (_attackType)
            {
                case AttackType.MeleeAttack:
                case AttackType.TouchSpellAttack:
                    if (distance > _actor.Speed)
                    {
                        addendum = $"Out of range ({distance} > {_actor.Speed})";
                        return false;
                    }

                    break;
                case AttackType.RangedAttack:
                    return RangedAttackConsiderations(distance, out addendum);
                case AttackType.RangedSpellAttack:
                    return RangedSpellConsiderations(distance, out addendum);
                case AttackType.DcAttack:
                    return RangedDcConsiderations(distance, out addendum);
                default:
                    throw new NotImplementedException("Heuristic: _attackType undefined");
            }

            return true;
        }

        private bool RangedDcConsiderations(float distance, out string reason)
        {
            reason = "";
            if (distance > _actor.Speed + _dcAttack.Reach)
            {
                reason = $"Out of range ({distance} > {_actor.Speed} + {_dcAttack.Reach}) ";
                return false;
            }

            return true;
        }

        private bool RangedAttackConsiderations(float distance, out string reason)
        {
            reason = "";
            if (distance < 2) AddDisadvantage();
            if (distance > _actor.Speed + _rangedAttack.LongRange)
            {
                reason = $"Out of range ({distance} > {_actor.Speed} +  {_rangedAttack.LongRange}) ";
                return false;
            }

            if (distance > _actor.Speed + _rangedAttack.ShortRange)
            {
                reason = $"Long range ({distance} > {_rangedAttack.LongRange})";
                AddDisadvantage();
            }

            return true;
        }

        public void NoRangeDisadvantage()
        {
            _rangeDisadvantage = false;
        }

        private bool RangedSpellConsiderations(float distance, out string reason)
        {
            reason = "";
            if (distance < 2 && _rangeDisadvantage) AddDisadvantage();
            if (distance > _actor.Speed + _spell.Reach)
            {
                reason = $"Out of range ({distance} > {_actor.Speed} + {_spell.Reach})";
                return false;
            }

            return true;
        }

        public void AddFriends(int numFriends)
        {
            _friends = numFriends;
        }

        public void AddEnemies(int numEnemies)
        {
            _enemies = numEnemies;
        }

        public void AddRepeat(int numTimes)
        {
            _repeats = numTimes;
        }

        public void AddDamageRoll(DamageRoll damageRoll)
        {
            _maxDamage = damageRoll.Roll(max: true).hits;
        }

        public void AddDamage(int damage)
        {
            _maxDamage = damage;
        }

        public void AddAdvantage(bool hasAdvantage = true)
        {
            _hasAdvantage = hasAdvantage;
        }

        public void AddDisadvantage(bool hasDisadvantage = true)
        {
            _hasDisadvantage = hasDisadvantage;
        }

        private enum AttackType
        {
            MeleeAttack,
            RangedAttack,
            RangedSpellAttack,
            TouchSpellAttack,
            DcAttack
        }
    }
}