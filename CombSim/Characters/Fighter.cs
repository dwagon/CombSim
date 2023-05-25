using System.Collections.Generic;
using CombSim.Gear;

namespace CombSim.Characters
{
    public class Fighter : Character
    {
        private readonly Dictionary<int, int> _hitPointsAtLevel = new Dictionary<int, int>()
        {
            { 1, 12 }, { 2, 20 }, { 3, 28 }, { 4, 36 }
        };

        public Fighter(string name, int level = 1, string team = "Fighters") : base(name, team)
        {
            Repr = "F";

            Level = level;
            MaxHitPoints = _hitPointsAtLevel[level];

            Stats.Add(StatEnum.Strength, new Stat(16));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(15));
            Stats.Add(StatEnum.Intelligence, new Stat(11));
            Stats.Add(StatEnum.Wisdom, new Stat(13));
            Stats.Add(StatEnum.Charisma, new Stat(9));
            AddEquipment(MeleeWeaponGear.Mace);
            AddEquipment(ArmourGear.Plate);
            AddEquipment(ArmourGear.Shield);
            AddEquipment(PotionsGear.HealingPotion);
            AddAction(new SecondWind());
            if (level >= 2)
            {
                AddAction(new ActionSurge());
            }

            if (level >= 3)
            {
                // Champion Martial Archetype
                CriticalHitRoll = 19;
            }

            if (level >= 4)
            {
                Stats[StatEnum.Strength] = new Stat(18);
            }
        }

        private class SecondWind : Action
        {
            public SecondWind() : base("Second Wind", ActionCategory.Bonus)
            {
            }

            public override int GetHeuristic(Creature actor, out string reason)
            {
                int result;

                if (actor.PercentHitPoints() > 50)
                {
                    reason = "Have most of our hitpoints";
                    return 0;
                }

                var magicFormula = actor.HitPointsDown() / 10 + 1;
                reason = $"Down {actor.HitPointsDown()} HP";
                result = magicFormula;

                return result;
            }

            public override void DoAction(Creature actor)
            {
                Fighter f = (Fighter)actor;
                actor.Heal(Dice.Roll("d10") + f.Level, "Second Wind");
                actor.RemoveAction(this);
            }
        }

        private class ActionSurge : Action
        {
            public ActionSurge() : base("Action Surge", ActionCategory.Supplemental)
            {
            }

            public override int GetHeuristic(Creature actor, out string reason)
            {
                // Only invoke it if we are next to an enemy
                var enemy = actor.PickClosestEnemy();
                if (actor.DistanceTo(enemy) < 2)
                {
                    reason = "Adjacent to enemy";
                    return 1;
                }

                reason = "Not close enough";
                return 0;
            }

            public override void DoAction(Creature actor)
            {
                NarrationLog.LogMessage($"{actor.Name} uses Action Surge");
                actor.DoActionCategory(ActionCategory.Action, force: true);
                actor.RemoveAction(this);
                base.DoAction(actor);
            }
        }
    }
}