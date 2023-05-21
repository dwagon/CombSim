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

            AddAction(new ElementalSlap());
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