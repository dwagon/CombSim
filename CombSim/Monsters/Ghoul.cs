// https://www.dndbeyond.com/monsters/ghoul

using System;

namespace CombSim.Monsters
{
    public class Ghoul : Monster
    {
        public Ghoul(string name, string team = "Ghouls") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(13));
            Stats.Add(StatEnum.Dexterity, new Stat(15));
            Stats.Add(StatEnum.Constitution, new Stat(10));
            Stats.Add(StatEnum.Intelligence, new Stat(7));
            Stats.Add(StatEnum.Wisdom, new Stat(10));
            Stats.Add(StatEnum.Charisma, new Stat(6));
            Repr = "G";
            HitDice = "5d8";
            ArmourClass = 12;
            Immune.Add(DamageTypeEnums.Poison);
            AddAction(new GhoulBite());
            AddAction(new GhoulClaw());
            ProficiencyBonus = 2;
        }

        private class GhoulBite : MeleeAttack
        {
            public GhoulBite() : base("Ghoul Bite", new DamageRoll("2d6", DamageTypeEnums.Piercing))
            {
            }

            public override int GetHeuristic(Creature actor)
            {
                return 2;
            }
        }

        private class GhoulClaw : MeleeAttack
        {
            public GhoulClaw() : base("Ghoul Claw", new DamageRoll("2d4", DamageTypeEnums.Slashing))
            {
            }

            public override int GetHeuristic(Creature actor)
            {
                return 1 + (actor.HasCondition(ConditionEnum.Paralyzed) ? 0 : 2);
            }

            protected override void SideEffect(Creature actor, Creature target)
            {
                Console.WriteLine($"// Ghoul Claw Side Effect");
                if (!target.MakeSavingThrow(StatEnum.Constitution, 10))
                {
                    target.AddEffect(new GhoulParalyzation(target));
                }
            }
        }

        private class GhoulParalyzation : Effect
        {
            public GhoulParalyzation(Creature target) : base("Ghoul Paralyzation")
            {
                target.OnTurnEnd += TurnEnd;
            }

            private void TurnEnd(Object sender, OnTurnEndEventArgs e)
            {
                if (e.Creature.MakeSavingThrow(StatEnum.Constitution, 10))
                {
                    Console.WriteLine("// Remove Effect Ghoul Para");
                    e.Creature.RemoveEffect(this);
                }
            }

            public override void Start(Creature target)
            {
                Console.WriteLine($"// {target} now has ghoul effect");
                target.AddCondition(ConditionEnum.Paralyzed);
            }

            public override void End(Creature target)
            {
                target.RemoveCondition(ConditionEnum.Paralyzed);
                target.OnTurnEnd -= TurnEnd;
            }
        }
    }
}