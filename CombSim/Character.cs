namespace CombSim
{
    public class Character : Creature
    {
        private int _deathSavesBad;
        private int _deathSavesGood;

        protected Character(string name, string team = "Characters") : base(name, team)
        {
            _deathSavesGood = 0;
            _deathSavesBad = 0;
        }

        protected int Level { get; set; }

        protected override void FallenUnconscious()
        {
            NarrationLog.LogMessage($"{Name} has fallen unconscious");
            Conditions.SetCondition(ConditionEnum.Unconscious);
            Conditions.RemoveCondition(ConditionEnum.Ok);
            HitPoints = 0;
            _deathSavesBad = 0;
            _deathSavesGood = 0;
        }

        protected override void TurnStart()
        {
            base.TurnStart();

            if (HasCondition(ConditionEnum.Unconscious) && !HasCondition(ConditionEnum.Stable) &&
                !HasCondition(ConditionEnum.Dead))
            {
                var roll = Dice.RollD20();

                if (roll == 1)
                {
                    _deathSavesBad += 2;
                    NarrationLog.LogMessage($"{Name} has badly failed a Death Saving Throw ({_deathSavesBad} Fails)");
                }
                else if (roll <= 10)
                {
                    _deathSavesBad++;
                    NarrationLog.LogMessage($"{Name} has failed a Death Saving Throw ({_deathSavesBad} Fails)");
                }
                else if (roll == 20)
                {
                    HitPoints = 1;
                    Conditions.RemoveCondition(ConditionEnum.Unconscious);
                    Conditions.SetCondition(ConditionEnum.Ok);
                    NarrationLog.LogMessage(
                        $"{Name} has critically succeeded at a Death Saving Throw and is now conscious");
                    _deathSavesGood = 0;
                    _deathSavesBad = 0;
                }
                else
                {
                    _deathSavesGood++;
                    NarrationLog.LogMessage(
                        $"{Name} has succeeded at a Death Saving Throw ({_deathSavesGood} Successes)");
                }

                if (_deathSavesBad >= 3) Died();
                if (_deathSavesGood >= 3) Conditions.SetCondition(ConditionEnum.Stable);
            }
        }
    }
}