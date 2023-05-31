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
            ArmourClass = 14;
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

        private class GiantSpiderWeb : Action
        {
            private bool _hasCharge = true;

            public GiantSpiderWeb() : base("Web", ActionCategory.Action)
            {
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
    }
}