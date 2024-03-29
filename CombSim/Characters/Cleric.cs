using System;
using System.Collections.Generic;
using CombSim.Gear;
using CombSim.Spells;
using Pastel;

namespace CombSim.Characters
{
    public class Cleric : Character
    {
        private readonly Dictionary<int, int> _channelDivinity = new Dictionary<int, int>
        {
            { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }
        };

        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>
        {
            { 1, 9 }, { 2, 15 }, { 3, 21 }, { 4, 27 }, { 5, 33 }
        };

        // CasterLevel: <SpellLevel: NumberOfSlots>
        private readonly Dictionary<int, Dictionary<int, int>> _spellsAtLevel =
            new Dictionary<int, Dictionary<int, int>>
            {
                { 1, new Dictionary<int, int> { { 1, 2 } } },
                { 2, new Dictionary<int, int> { { 1, 3 } } },
                { 3, new Dictionary<int, int> { { 1, 4 }, { 2, 2 } } },
                { 4, new Dictionary<int, int> { { 1, 4 }, { 2, 3 } } },
                { 5, new Dictionary<int, int> { { 1, 4 }, { 2, 3 }, { 3, 2 } } }
            };

        public Cleric(string name, int level = 1, string team = "Clerics") : base(name, level, team)
        {
            Repr = "C".Pastel(ConsoleColor.Red).PastelBg(ConsoleColor.DarkBlue);

            Level = level;
            MaxHitPoints = _hitPointsAtLevel[level];
            SpellCastingAbility = StatEnum.Wisdom;

            Stats.Add(StatEnum.Strength, new Stat(15));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(13));
            Stats.Add(StatEnum.Intelligence, new Stat(9));
            Stats.Add(StatEnum.Wisdom, new Stat(16));
            Stats.Add(StatEnum.Charisma, new Stat(11));
            AddEquipment(MeleeWeaponGear.Flail);
            AddEquipment(ArmourGear.Ring);
            AddEquipment(new HealingPotion());

            // Cantrips
            AddSpell(new SacredFlame(Level));
            // AddSpell(new Guidance());
            // AddSpell(new SpareTheDying());

            // Level 1
            // AddSpell(new Bane());
            // AddSpell(new Bless());       // Life Domain
            // AddSpell(new CureWounds());  // Life Domain
            AddSpell(new GuidingBolt(Level));
            AddSpell(new HealingWord(Level));
            AddSpell(new InflictWounds(Level));

            if (Level >= 2)
            {
                AddAction(new PreserveLife(this));
                // AddAction(new TurnUndead());
            }

            if (Level >= 3)
            {
                AddEquipment(new ArmourGear.ArrowCatchingShield());
            }
            else
            {
                AddEquipment(ArmourGear.Shield);
            }

            if (level >= 4)
            {
                Stats[StatEnum.Wisdom] = new Stat(18);
            }

            if (level >= 5)
            {
                AddSpell(new MassHealingWord(Level));
                AddSpell(new SpiritGuardians(Level));
                AddEquipment(new GreaterHealingPotion());
            }
        }

        // Disciple of Life: Healing Spells cure addition 2+Level HP
        public override int HealingBonus()
        {
            return 2 + Level;
        }

        public override string ToString()
        {
            var baseString = base.ToString();
            var spellString = "; Spells: ";
            foreach (var kvp in _spellsAtLevel[Level])
            {
                spellString += $"L{kvp.Key} = {kvp.Value}; ";
            }

            var channelString = $"ChDiv: {_channelDivinity[Level]};";

            return baseString + spellString + channelString;
        }

        public override bool CanCastSpell(Spell spell)
        {
            if (spell.Level == 0) return true;
            if (_spellsAtLevel[Level][spell.Level] >= 1) return true;
            return false;
        }

        public override void DoCastSpell(Spell spell)
        {
            Console.WriteLine($"// DoCastSpell(spell={spell.Name()}) Level: {spell.Level}");
            if (spell.Level == 0) return;
            _spellsAtLevel[Level][spell.Level]--;
        }

        private bool CanChannelDivinity()
        {
            return _channelDivinity[Level] > 0;
        }

        private void DoChannelDivinity()
        {
            _channelDivinity[Level]--;
        }

        private class ChannelDivinity : Action
        {
            protected ChannelDivinity(string name) : base(name, ActionCategory.Action)
            {
            }

            protected override void DoAction(Creature actor)
            {
                var cleric = (Cleric)actor;
                cleric.DoChannelDivinity();
            }
        }

        private class PreserveLife : ChannelDivinity
        {
            private readonly int _maxHpToHeal;
            private readonly int _reach;

            public PreserveLife(Cleric actor) : base("Preserve Life")
            {
                _reach = 30 / 5;
                _maxHpToHeal = actor.Level * 5;
            }

            private List<Creature> NearbyAlliesNeedingHealing(Creature actor, out int hpToHeal)
            {
                hpToHeal = 0;
                var allAlliesNeedingHealing = new List<Creature>();

                foreach (var friend in actor.GetAllAllies())
                {
                    if (friend.HitPointsDown() > 0 && actor.DistanceTo(friend) <= _reach)
                    {
                        allAlliesNeedingHealing.Add(friend);
                        hpToHeal += friend.HitPointsDown();
                    }
                }

                return allAlliesNeedingHealing;
            }

            public override int GetHeuristic(Creature actor, out string reason)
            {
                var cleric = (Cleric)actor;
                if (!cleric.CanChannelDivinity())
                {
                    reason = "No Channel Divinities left";
                    return 0;
                }

                var allAlliesNeedingHealing = NearbyAlliesNeedingHealing(actor, out int hpToHeal);
                if (allAlliesNeedingHealing.Count == 0)
                {
                    reason = "No one needs healing";
                    return 0;
                }

                hpToHeal = Math.Min(hpToHeal, _maxHpToHeal);
                reason = $"Can cure {allAlliesNeedingHealing.Count} ally of {hpToHeal} HP";

                return Math.Min(_maxHpToHeal, hpToHeal);
            }

            protected override void DoAction(Creature actor)
            {
                var cleric = (Cleric)actor;
                cleric.DoChannelDivinity();
                var allAlliesNeedingHealing = NearbyAlliesNeedingHealing(actor, out int _);
                var hpHealed = 0;
                foreach (var creature in allAlliesNeedingHealing)
                {
                    var hpToHeal = Math.Min(creature.HitPointsDown(), _maxHpToHeal - hpHealed) /
                                   allAlliesNeedingHealing.Count;
                    creature.Heal(hpToHeal, "Preserve Life");
                    hpHealed += hpToHeal;
                }
            }
        }

        private class TurnUndead : ChannelDivinity
        {
            public TurnUndead() : base("Turn Undead")
            {
            }

            public override int GetHeuristic(Creature actor, out string reason)
            {
                reason = "Unimplemented";
                return 0;
            }
        }
    }
}