namespace CombSim.Characters
{
    public class Fighter : Character
    {
        public Fighter(string name, string team = "Fighters") : base(name, team)
        {
            Repr = "F";
            MaxHitPoints = 12;
            Level = 1;

            Stats.Add(StatEnum.Strength, new Stat(16));
            Stats.Add(StatEnum.Dexterity, new Stat(14));
            Stats.Add(StatEnum.Constitution, new Stat(15));
            Stats.Add(StatEnum.Intelligence, new Stat(11));
            Stats.Add(StatEnum.Wisdom, new Stat(13));
            Stats.Add(StatEnum.Charisma, new Stat(9));
            AddEquipment(Gear.Mace);
            AddEquipment(Gear.Plate);
            AddEquipment(Gear.Shield);
            AddAction(new SecondWind());
        }

        private class SecondWind : Action
        {
            public SecondWind() : base("Second Wind", ActionCategory.Bonus)
            {
            }

            public override int GetHeuristic(Creature actor)
            {
                int result;
        
                 if (actor.PercentHitPoints() > 50)
                {
                    result = 0;
                }
                else
                {
                    result = actor.HitPointsDown() / 10 + 1;
                }

                return result;
            }

            public override bool DoAction(Creature actor)
            {
                Fighter f = (Fighter)actor;
                var cured = actor.Heal(Dice.Roll("d10") + f.Level);
                NarrationLog.LogMessage($"{actor.Name} uses Second Wind to cure themselves {cured} HP");
                actor.RemoveAction(this);
                return true;
            }
        }
    }
}