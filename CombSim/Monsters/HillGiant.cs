namespace CombSim.Monsters
{
    public class HillGiant : Monster
    {
        public HillGiant(string name = "Hill Giant", string team = "Hill Giants") : base(name, team)
        {
            Stats.Add(StatEnum.Strength, new Stat(21));
            Stats.Add(StatEnum.Dexterity, new Stat(8));
            Stats.Add(StatEnum.Constitution, new Stat(19));
            Stats.Add(StatEnum.Intelligence, new Stat(5));
            Stats.Add(StatEnum.Wisdom, new Stat(9));
            Stats.Add(StatEnum.Charisma, new Stat(6));
            Repr = "G";
            HitDice = "10d12+40";
            ArmourClass(13);
            ProficiencyBonus = 3;

            var multiAttack = new MultiAttack("Multi Club Attack", ActionCategory.Action);
            multiAttack.AddAttack(new GiantClub());
            multiAttack.AddAttack(new GiantClub());
            AddAction(multiAttack);
            AddEquipment(new GiantRock());
        }

        private class GiantClub : MeleeWeapon
        {
            public GiantClub() : base("Greatclub", new DamageRoll("3d8", DamageTypeEnums.Bludgeoning), 10 / 5)
            {
            }
        }

        private class GiantRock : RangedWeapon
        {
            public GiantRock() : base("Rock", new DamageRoll("3d10", DamageTypeEnums.Bludgeoning), 60 / 5, 240 / 5)
            {
                Thrown = true;
            }
        }
    }
}