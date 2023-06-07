using System;
using Pastel;

namespace CombSim.Monsters
{
    public class GiantSpider : Monster
    {
        public GiantSpider(string name, string team = "Bugbears") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(14));
            Stats.Add(StatEnum.Dexterity, new Stat(16));
            Stats.Add(StatEnum.Constitution, new Stat(12));
            Stats.Add(StatEnum.Intelligence, new Stat(2));
            Stats.Add(StatEnum.Wisdom, new Stat(11));
            Stats.Add(StatEnum.Charisma, new Stat(4));
            Repr = "S".Pastel(ConsoleColor.Black).PastelBg(ConsoleColor.White);
            HitDice = "4d10+4";
            ArmourClass(14);
            ProficiencyBonus = 2;
            AddAction(new GiantSpiderBite());
            var web = new GiantSpiderWeb();
            OnTurnStart += web.TurnStart;
            AddAction(web);
        }

        private class GiantSpiderBite : MeleeAttack
        {
            public GiantSpiderBite() : base("Spider Bite", new DamageRoll("1d8", DamageTypeEnums.Piercing))
            {
                Finesse = true;
            }

            // Melee Weapon Attack: +5 to hit, reach 5 ft., one creature. Hit: 7 (1d8 + 3) piercing damage,
            // and the target must make a DC 11 Constitution saving throw, taking 9 (2d8) poison damage on a failed save,
            // or half as much damage on a successful one.
            protected override void SideEffect(Creature actor, Creature target)
            {
                var attack = new GiantSpiderBiteSideEffect();
                attack.DoAttack(actor, target);
            }
        }

        private class GiantSpiderBiteSideEffect : DcAttack
        {
            public GiantSpiderBiteSideEffect() : base("Spider Bite Poison", StatEnum.Constitution, 11,
                new DamageRoll("2d8", DamageTypeEnums.Poison), SpellSavedEffect.DamageHalved)
            {
            }
        }

        // Web (Recharge 5–6). Ranged Weapon Attack: +5 to hit, range 30/60 ft., one creature.
        // Hit: The target is restrained by webbing. As an action, the restrained target can make a DC 12 Strength check,
        // bursting the webbing on a success.
        private class GiantSpiderWeb : RangedAttack
        {
            private bool _hasCharge = true;

            public GiantSpiderWeb() : base("Web", null, 30 / 5, 60 / 5)
            {
            }

            protected override void SideEffect(Creature actor, Creature target)
            {
                target.AddEffect(new Webbed());
            }

            public override int GetHeuristic(Creature actor, out string reason)
            {
                var enemy = actor.PickClosestEnemy();
                var distance = actor.DistanceTo(enemy);
                if (distance > LongRange + actor.Speed)
                {
                    reason = "Beyond movement";
                    return 0;
                }

                if (enemy.HasEffect("Giant Spider Web"))
                {
                    reason = $"{enemy.Name} already webbed";
                    return 0;
                }

                reason = $"Can web {enemy.Name} ({distance})";
                return 10;
            }

            public void TurnStart(object sender, OnTurnStartEventArgs e)
            {
                if (_hasCharge) return;

                var roll = Dice.Roll("1d6");
                if (roll >= 5)
                {
                    _hasCharge = true;
                }
            }
        }

        private class Webbed : Effect
        {
            private Action _action;

            public Webbed() : base("Giant Spider Web")
            {
            }

            public override void Start(Creature target)
            {
                Console.WriteLine($"// {target.Name} is entrapped in a web");
                target.AddCondition(ConditionEnum.Restrained);
                _action = new RemoveWeb(this);
                target.AddAction(_action);
            }

            public override void End(Creature target)
            {
                target.RemoveCondition(ConditionEnum.Restrained);
                target.RemoveAction(_action);
            }

            private class RemoveWeb : Action
            {
                private readonly Effect _webbed;

                public RemoveWeb(Effect webEffect) : base("Remove Web", ActionCategory.Action)
                {
                    _webbed = webEffect;
                }

                public override int GetHeuristic(Creature actor, out string reason)
                {
                    reason = "Remove web";
                    return 15;
                }

                public override void DoAction(Creature actor)
                {
                    if (actor.MakeSavingThrow(StatEnum.Strength, 12, out _))
                    {
                        actor.RemoveEffect(_webbed);
                    }
                }
            }
        }
    }
}