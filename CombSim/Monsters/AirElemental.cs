namespace CombSim.Monsters
{
    public class AirElemental : Monster
    {
        public AirElemental(string name, string team = "Elementals") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(14));
            Stats.Add(StatEnum.Dexterity, new Stat(20));
            Stats.Add(StatEnum.Constitution, new Stat(14));
            Stats.Add(StatEnum.Intelligence, new Stat(6));
            Stats.Add(StatEnum.Wisdom, new Stat(10));
            Stats.Add(StatEnum.Charisma, new Stat(6));
            Repr = "A";
            HitDice = "12d10+24";
            ArmourClass = 15;
            Resistant.Add(DamageTypeEnums.Lightning);
            Resistant.Add(DamageTypeEnums.Thunder);
            Resistant.Add(DamageTypeEnums.Bludgeoning);
            Resistant.Add(DamageTypeEnums.Piercing);
            Resistant.Add(DamageTypeEnums.Slashing);
            Immune.Add(DamageTypeEnums.Poison);
            ProficiencyBonus = 3;
            Speed = 90 / 5;

            var multiAttack = new MultiAttack("Multi Attack", ActionCategory.Action);
            multiAttack.AddAttack(new ElementalSlap());
            multiAttack.AddAttack(new ElementalSlap());

            AddAction(multiAttack);
            var whirlWind = new Whirlwind();
            AddAction(whirlWind);
            this.OnTurnStart += whirlWind.TurnStart;
        }

        private class Whirlwind : DcAttack
        {
            private bool _hasCharge;

            public Whirlwind() : base("Whirlwind", StatEnum.Strength, 13,
                new DamageRoll("3d8+2", DamageTypeEnums.Bludgeoning), SpellSavedEffect.DamageHalved)
            {
                _hasCharge = true;
            }

            protected override void FailSideEffect(Creature actor, Creature target)
            {
                target.AddCondition(ConditionEnum.Prone);
            }

            public void TurnStart(object sender, OnTurnStartEventArgs e)
            {
                if (!_hasCharge)
                {
                    var roll = Dice.Roll("1d6");
                    if (roll >= 4)
                    {
                        _hasCharge = true;
                    }
                }
            }

            public override int GetHeuristic(Creature actor)
            {
                var enemy = actor.PickClosestEnemy();
                if (enemy == null) return 0;
                if (!_hasCharge) return 0;
                if (actor.DistanceTo(enemy) <= 2) return 4;
                if (actor.DistanceTo(enemy) <= 2 + actor.Speed) return 3;
                return 1;
            }

            public override void DoAction(Creature actor)
            {
                var enemy = actor.PickClosestEnemy();
                if (enemy == null) return;
                actor.MoveWithinReachOfEnemy(1, enemy);

                if (actor.Game.DistanceTo(actor, enemy) <= 1)
                {
                    DoAttack(actor, enemy);
                    _hasCharge = false;
                }
            }
        }

        private class ElementalSlap : MeleeAttack
        {
            public ElementalSlap() : base("Slap", new DamageRoll("2d8", DamageTypeEnums.Bludgeoning))
            {
                Versatile = true;
            }
        }
    }
}