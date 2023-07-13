namespace CombSim.Monsters
{
    public class Ape : Monster
    {
        public Ape(string name, string team = "Apes") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(16));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(14));
            Stats.Add(StatEnum.Intelligence, new Stat(6));
            Stats.Add(StatEnum.Wisdom, new Stat(12));
            Stats.Add(StatEnum.Charisma, new Stat(7));
            Repr = "A";
            HitDice = "3d8+6";
            ArmourClass(12);
            ProficiencyBonus = 2;

            AddAction(new ApeRock());
            var multiAttack = new MultiAttack("Fists", ActionCategory.Action);
            multiAttack.AddAttack(new ApeFist());
            multiAttack.AddAttack(new ApeFist());
            AddAction(multiAttack);
        }

        private class ApeFist : MeleeAttack
        {
            public ApeFist() : base("Fist", new DamageRoll("1d6", DamageTypeEnums.Bludgeoning))
            {
            }
        }

        private class ApeRock : RangedAttack
        {
            public ApeRock() : base("Rock", new DamageRoll("1d6", DamageTypeEnums.Bludgeoning), 25 / 5, 50 / 5)
            {
            }
        }
    }
}